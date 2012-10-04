#ifndef GPIO_UDP_H_
#define GPIO_UDP_H_

#include <EthernetUdp.h>

#include "log.h"
#include "gpio_events.h"

namespace gpio {

template<class Handler>
class udp_listener
{
  EthernetUDP& udp_;
  Handler& handler_;
  unsigned char packet_buffer[UDP_TX_PACKET_MAX_SIZE];
public:
  udp_listener(EthernetUDP& udp, Handler& handler)
    : udp_(udp)
    , handler_(handler)
  {
  }

  void check_incoming()
  {
    int num_bytes = udp_.parsePacket();
    
    if (num_bytes == 0)
      return;

    unsigned char event_id = udp_.read();
    
    if (event_id == -1)
    {
      log("Unknown event id");

      return;
    }

    unsigned char required_length = handler_.get_num_bytes(event_id);

    if (required_length != num_bytes - 1)
    {
      log("Invalid packet length");

      return;
    }
    
    udp_.read(packet_buffer, required_length);
    
    handler_.handle(event_id, packet_buffer);
  }
};

class udp_sender
{
  EthernetUDP& udp_;
  unsigned char packet_buffer[UDP_TX_PACKET_MAX_SIZE];
public:
  udp_sender(EthernetUDP& udp)
    : udp_(udp)
  {
  }
  
  template<class Event>
  void send(const Event& event)
  {
    typedef event_marshaller<Event> marshaller;
    
    udp_.beginPacket(udp_.remoteIP(), udp_.remotePort());
    
    udp_.write(marshaller::get_event_id());
    marshaller::marshal(event, packet_buffer);
    udp_.write(packet_buffer, marshaller::get_num_bytes());
  }
};

}

#endif
