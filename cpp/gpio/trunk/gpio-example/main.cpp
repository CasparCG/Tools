#include <iostream>

#include <boost/bind.hpp>
#include <boost/thread/future.hpp>

#include <gpio/serial_port_device.h>

int main()
{
    try
    {
        gpio::gpio_device::ptr device =
                gpio::serial_port_device::create("COM1", 115200);

        std::cout
                << device->get_description() << " with "
                << device->get_num_gpi_ports() << " GPI and "
                << device->get_num_gpo_ports() << " GPO and min_duration "
                << device->get_minimum_supported_duration() << std::endl;

        gpio::gpo_switch::ptr tally_switch =
                device->setup_gpo_tally(0, gpio::LOW);
        device->setup_gpi_tally(0, gpio::LOW, boost::bind(
                &gpio::gpo_switch::set, tally_switch, _1));
        boost::promise<void> quit_promise;
        boost::unique_future<void> quit = quit_promise.get_future();
        device->setup_gpi_pulse(1, gpio::LOW,
                boost::bind(&boost::promise<void>::set_value, &quit_promise));

        /*auto tally_1 = device->setup_gpo_tally(1, gpio::LOW);
        device->setup_gpi_tally(1, gpio::HIGH, [&] (bool state)
        {
            std::cout << "Tally: " << (state ? "on" : "off") << std::endl;
            tally_1->set(state);
        });

        auto trigger_0 = device->setup_gpo_pulse(0, gpio::LOW, 50);
        device->setup_gpi_pulse(0, gpio::HIGH, [&]
        {
            std::cout << "Pulse" << std::endl;
            trigger_0->fire();
        });

        gpio::separated_switch_handler tally_handler(
            [=]
            {
                tally_1->set(false);
            },
            [=]
            {
                tally_1->set(true);
            });

        device->setup_gpi_tally(0, gpio::LOW, tally_handler);*/

        quit.get();
    }
    catch (const std::exception& e)
    {
        std::cerr << e.what() << std::endl;

        return 1;
    }

    return 0;
}

