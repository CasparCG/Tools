#include "gpio_events.h"
//#include "gpio_udp.h"
#include "gpio_stream_based.h"
#include "gpio_handler.h"
#include "event_handlers.h"

//#include <Ethernet.h>
//#include <EthernetUdp.h>
//#include <SPI.h>

const int LOW_PIN = 2;
const int HIGH_PIN = 9;
int current_pin = LOW_PIN;

byte mac[] =
  {
    0xDE,
    0xAD,
    0xBE,
    0xEF,
    0xFE
  };
//IPAddress ip(10, 21, 81, 220);

//EthernetUDP udp;

gpio::gpio_handler handler;
//gpio::udp_listener<gpio::gpio_handler> listener(udp, handler);
//gpio::udp_sender sender(udp);
gpio::stream_listener<gpio::gpio_handler> listener(Serial, handler);
gpio::stream_sender sender(Serial);

gpio::setup_gpio_pulse_port_handler<gpio::stream_sender> setup_gpio_pulse_port_handler(sender);

void setup()
{
  //boolean success = Ethernet.begin(mac) == 1;

  for (int i = LOW_PIN; i <= HIGH_PIN; ++i)
    pinMode(i, OUTPUT);

  handler.register_handler(&setup_gpio_pulse_port_handler);
  
  Serial.begin(9600);
}

void loop()
{
  int id = gpio::event_marshaller<gpio::setup_gpio_pulse_port>::get_event_id();
  digitalWrite(current_pin, LOW);

  if (++current_pin > HIGH_PIN)
    current_pin = LOW_PIN;
  
  digitalWrite(current_pin, HIGH);
  delay(125);
  
  listener.check_incoming();
}

