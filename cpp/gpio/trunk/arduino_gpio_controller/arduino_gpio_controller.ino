#include "simple_stream_based.h"

int BAUD_RATE = 9600;

typedef gpio::offset_port_mapper<2, 8> gpo_mapper_t;
typedef gpio::composed_mapper<
    gpio::offset_port_mapper<10, 2>,
    gpio::offset_port_mapper<14, 6>
  > gpi_mapper_t;

gpo_mapper_t gpo_mapper;
gpi_mapper_t gpi_mapper;

gpio::simple_listener<gpo_mapper_t> listener(Serial, gpo_mapper, gpi_mapper_t::NUM_PORTS);
gpio::simple_sender<gpi_mapper_t> sender(Serial, gpi_mapper);

void setup()
{
  while (!Serial) { }
  Serial.begin(BAUD_RATE);
}

void loop()
{
  listener.tick();
  sender.tick();

  Serial.flush();
}

