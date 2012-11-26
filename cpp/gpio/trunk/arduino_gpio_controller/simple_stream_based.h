#ifndef SIMPLE_STREAM_BASED_H_
#define SIMPLE_STREAM_BASED_H_

#include "Arduino.h"

namespace gpio {

template<int Offset, int NumPorts>
class offset_port_mapper
{
public:
  enum
  {
    NUM_PORTS = NumPorts
  };

  int to_hardware_port(int logical_port) const
  {
    if (logical_port < 0 || logical_port > NUM_PORTS)
      return -1;

    return logical_port + Offset;
  }
};

template<class Mapper1, class Mapper2>
class composed_mapper
{
  Mapper1 mapper1_;
  Mapper2 mapper2_;
public:
  enum
  {
    NUM_PORTS = Mapper1::NUM_PORTS + Mapper2::NUM_PORTS
  };

  int to_hardware_port(int logical_port) const
  {
    if (logical_port < 0 || logical_port > NUM_PORTS)
      return -1;
    
    if (logical_port < Mapper1::NUM_PORTS)
      return mapper1_.to_hardware_port(logical_port);
    else
      return mapper2_.to_hardware_port(logical_port - Mapper1::NUM_PORTS);
  }
};

template<class GpoMapper>
class simple_listener
{
  Stream& stream_;
  const GpoMapper& gpo_mapper_;
  int num_gpi_;
public:
  simple_listener(
      Stream& stream, const GpoMapper& gpo_mapper, int num_gpi)
    : stream_(stream)
    , gpo_mapper_(gpo_mapper)
    , num_gpi_(num_gpi)
  {
    for (int i = 0; i < GpoMapper::NUM_PORTS; ++i)
      pinMode(gpo_mapper_.to_hardware_port(i), OUTPUT);
  }

  void tick()
  {
    int num_bytes;
    
    while ((num_bytes = stream_.available()) >= 4)
    {
      int byte1 = stream_.read();
      int byte2 = stream_.read();
      
      // consume \r\n
      stream_.read();
      stream_.read();
      
      if (byte1 == 'i' && byte2 == '?')
      {
        send_num_inputs();

        continue;
      }
      else if (byte1 == 'o' && byte2 == '?')
      {
        send_num_outputs();

        continue;
      }
      else if (byte1 == 'a' && byte2 == '?')
      {
        send_alive();

        continue;
      }
      
      int port = byte1 - '0';
      bool state = byte2 - '0' == 1;
      int hardware_port = gpo_mapper_.to_hardware_port(port);
      
      if (hardware_port == -1)
        continue;
      
      digitalWrite(hardware_port, state ? HIGH : LOW);
    }
  }
  
  void send_num_inputs()
  {
    stream_.write('i');
    stream_.write('0' + num_gpi_);
    stream_.write('\r');
    stream_.write('\n');
  }
  
  void send_num_outputs()
  {
    stream_.write('o');
    stream_.write('0' + GpoMapper::NUM_PORTS);
    stream_.write('\r');
    stream_.write('\n');
  }
  
  void send_alive()
  {
    stream_.write('a');
    stream_.write('!');
    stream_.write('\r');
    stream_.write('\n');
  }
};

template<class GpiMapper>
class simple_sender
{
  Stream& stream_;
  const GpiMapper& gpi_mapper_;
  bool last_states_[GpiMapper::NUM_PORTS];
public:
  simple_sender(Stream& stream, const GpiMapper& gpi_mapper)
    : stream_(stream)
    , gpi_mapper_(gpi_mapper)
  {
    for (int i = 0; i < GpiMapper::NUM_PORTS; ++i)
    {
      pinMode(gpi_mapper_.to_hardware_port(i), INPUT);
      last_states_[i] = read_state(i);
    }
  }

  void tick()
  {
    for (int i = 0; i < GpiMapper::NUM_PORTS; ++i)
      send_if_changed(i);
  }
  
  void send_if_changed(int logical_port)
  {
    bool last = last_states_[logical_port];
    bool current = read_state(logical_port);
    
    if (last == current)
      return;
    
    last_states_[logical_port] = current;
    
    stream_.write(logical_port + '0');
    stream_.write(current ? '1' : '0');
    stream_.write('\r');
    stream_.write('\n');
  }
private:
  bool read_state(int logical_port)
  {
    int hardware_port = gpi_mapper_.to_hardware_port(logical_port);

    return digitalRead(hardware_port) == HIGH;
  }
};

}

#endif

