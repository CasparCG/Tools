#ifndef GPIO_EVENTS_H_
#define GPIO_EVENTS_H_

namespace gpio {

template<class T>
struct event_marshaller
{
};

enum gpio_type
{
  GPI,
  GPO
};

enum gpio_pulse_type
{
  RISING_EDGE,
  FALLING_EDGE
};

enum off_state
{
  LOW,
  HIGH
};

struct setup_gpio_pulse_port
{
  gpio_type type;
  gpio_pulse_type pulse_type;
  unsigned char pulse_duration_millis;
  unsigned char port;
};

template<>
struct event_marshaller<setup_gpio_pulse_port>
{
  static unsigned char get_event_id() { return 0; }
  static unsigned char get_num_bytes() { return 4; }
  static void unmarshal(setup_gpio_pulse_port& event, unsigned char* data)
  {
    event.type = static_cast<gpio_type>(data[0]);
    event.pulse_type = static_cast<gpio_pulse_type>(data[1]);
    event.pulse_duration_millis = data[2];
    event.port = data[3];
  }
};

struct setup_gpio_tally_port
{
  gpio_type type;
  off_state off;
};

template<>
struct event_marshaller<setup_gpio_tally_port>
{
  static unsigned char get_event_id() { return 1; }
  static unsigned char get_num_bytes() { return 2; }
  static void unmarshal(setup_gpio_tally_port& event, unsigned char* data)
  {
    event.type = static_cast<gpio_type>(data[0]);
    event.off = static_cast<off_state>(data[1]);
  }
};

struct pulse
{
  unsigned char port;
};

template<>
struct event_marshaller<pulse>
{
  static unsigned char get_event_id() { return 2; }
  static unsigned char get_num_bytes() { return 1; }
  static void unmarshal(pulse& event, unsigned char* data)
  {
    event.port = data[0];
  }

  static void marshal(const pulse& event, unsigned char* data)
  {
    data[0] = event.port;
  }
};

struct tally
{
  bool state;
  unsigned char port;
};

template<>
struct event_marshaller<tally>
{
  static unsigned char get_event_id() { return 3; }
  static unsigned char get_num_bytes() { return 2; }
  static void unmarshal(tally& event, unsigned char* data)
  {
    event.state = data[0] == 1;
    event.port = data[0];
  }

  static void marshal(const tally& event, unsigned char* data)
  {
    data[0] = event.state ? 1 : 0;
    data[1] = event.port;
  }
};

struct setup_ack
{
  unsigned char port;
  
  setup_ack(unsigned char port) : port(port) { }
};

template<>
struct event_marshaller<setup_ack>
{
  static unsigned char get_event_id() { return 4; }
  static unsigned char get_num_bytes() { return 1; }
  static void marshal(const setup_ack& event, unsigned char* data)
  {
    data[0] = event.port;
  }
};

struct setup_needed
{
  unsigned char port;
};

template<>
struct event_marshaller<setup_needed>
{
  static unsigned char get_event_id() { return 5; }
  static unsigned char get_num_bytes() { return 1; }
  static void marshal(const setup_needed& event, unsigned char* data)
  {
    data[0] = event.port;
  }
};

struct port_misconfigured
{
  unsigned char port;
};

template<>
struct event_marshaller<port_misconfigured>
{
  static unsigned char get_event_id() { return 6; }
  static unsigned char get_num_bytes() { return 1; }
  static void marshal(const port_misconfigured& event, unsigned char* data)
  {
    data[0] = event.port;
  }
};

struct still_alive
{
};

template<>
struct event_marshaller<still_alive>
{
  static unsigned char get_event_id() { return 7; }
  static unsigned char get_num_bytes() { return 0; }
  static void marshal(const still_alive& event, unsigned char* data)
  {
  }
};

}

#endif
