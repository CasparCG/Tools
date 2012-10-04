#ifndef EVENT_HANDLERS_H_
#define EVENT_HANDLERS_H_

#include "gpio_handler.h"
#include "gpio_events.h"

namespace gpio {

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

