#ifndef GPIO_STREAM_BASED_H_
#define GPIO_STREAM_BASED_H_

#include <Arduino.h>

#include "gpio_events.h"

namespace gpio {
  
#define NO_CURRENT_EVENT_ID 255

template<class Handler>
class stream_listener
{
  Stream& stream_;
  Handler& handler_;
  unsigned char current_event_id_;
  unsigned char event_data_[255];
public:
  stream_listener(Stream& stream, Handler& handler)
    : stream_(stream)
    , handler_(handler)
    , current_event_id_(NO_CURRENT_EVENT_ID)
  {
  }

  void check_incoming()
  {
    int num_bytes;
    
    while ((num_bytes = stream_.available()) > 0)
    {
      if (current_event_id_ == NO_CURRENT_EVENT_ID)
      {
        current_event_id_ = stream_.read();
      }
      else
      {
        unsigned char expected_length = handler_.get_num_bytes(current_event_id_);
        
        if (num_bytes >= expected_length)
        {
          stream_.readBytes(reinterpret_cast<char*>(event_data_), expected_length);
          
          handler_.handle(current_event_id_, event_data_);
          
          current_event_id_ = NO_CURRENT_EVENT_ID;
        }
        else
        {
          // Return and wait until more bytes are available.
          break;
        }
      }
    }
  }
};

class stream_sender
{
  Stream& stream_;
  unsigned char event_data_[255];
public:
  stream_sender(Stream& stream)
    : stream_(stream)
  {
  }
  
  template<class Event>
  void send(const Event& event)
  {
    typedef event_marshaller<Event> marshaller;
    
    stream_.write(marshaller::get_event_id());
    marshaller::marshal(event, event_data_);
    stream_.write(event_data_, marshaller::get_num_bytes());
  }
};

}

#endif
