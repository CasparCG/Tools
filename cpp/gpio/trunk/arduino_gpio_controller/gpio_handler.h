#ifndef GPIO_HANDLER_H_
#define GPIO_HANDLER_H_

#include "log.h"
#include "gpio_events.h"

namespace gpio {

class event_handler_base
{
public:
  virtual ~event_handler_base() { }
  virtual unsigned char get_num_bytes() = 0;
  virtual void handle(unsigned char* bytes) = 0;
};

template<class Event>
class event_handler : public event_handler_base
{
public:
  typedef Event event_type;
  
  virtual unsigned char get_num_bytes()
  {
    return event_marshaller<Event>::get_num_bytes();
  }

  virtual void handle(unsigned char* bytes)
  {
    Event event;
    
    event_marshaller<Event>::unmarshal(event, bytes);
    
    handle(event);
  }
  
  virtual void handle(const Event& event) = 0;
};

#define MAX_EVENT_ID 10

class gpio_handler
{
  event_handler_base* handlers_by_event_id[MAX_EVENT_ID];
public:
  gpio_handler()
  {
    for (int i = 0; i < MAX_EVENT_ID; ++i)
    {
      handlers_by_event_id[i] = 0;
    }
  }
  
  unsigned char get_num_bytes(unsigned char event_id) const
  {
    handlers_by_event_id[event_id]->get_num_bytes();
  }
  
  template<class EventHandler>
  void register_handler(EventHandler* handler)
  {
    typedef event_marshaller<typename EventHandler::event_type> marshaller;

    unsigned char event_id = marshaller::get_event_id();
    
    if (event_id > MAX_EVENT_ID)
    {
      log("Max event id to low");

      return;
    }
    
    handlers_by_event_id[event_id] = handler;
  }

  void handle(unsigned char event_id, unsigned char* packet_buffer)
  {
    if (event_id > MAX_EVENT_ID)
    {
      log("Event id too high");
      
      return;
    }
    
    event_handler_base* handler = handlers_by_event_id[event_id];
    
    if (handler == 0)
    {
      log("No handler registered for event id");
      
      return;
    }
    
    handler->handle(packet_buffer);
  }
};

}

#endif

