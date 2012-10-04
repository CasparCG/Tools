#ifndef EVENT_HANDLERS_H_
#define EVENT_HANDLERS_H_

#include "gpio_handler.h"
#include "gpio_events.h"

namespace gpio {

struct port
{
  bool setup;
  gpio_type type;
  gpio_pulse_type pulse_type;
  bool on;
  unsigned long millis_since_change;
  byte duration_millis;
  
  port()
    : setup(false)
    , type(GPI)
    , pulse_type(RISING_EDGE)
    , on(false)
    , millis_since_change(0L)
    , duration_millis(0)
  {
  }
}

#define NUM_PORTS 16;

struct ports_context
{
  port ports[NUM_PORTS];
};

template<class Sender>
class setup_gpio_pulse_port_handler : public event_handler<setup_gpio_pulse_port>
{
  Sender& sender_;
public:
  setup_gpio_pulse_port_handler(Sender& sender)
    : sender_(sender)
  {
  }

  virtual void handle(const setup_gpio_pulse_port& event)
  {
    sender_.send(setup_ack(event.port));
  }
};

}

#endif

