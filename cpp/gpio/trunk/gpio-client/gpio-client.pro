TARGET = gpio-client
TEMPLATE = lib
CONFIG -= console
CONFIG -= qt
CONFIG += rtti
CONFIG += exceptions
CONFIG += c++11

SOURCES += \
    src/serial_port_device.cpp

HEADERS += \
    include/gpio/serial_port_device.h \
    include/gpio/gpio_device.h

INCLUDEPATH += include
INCLUDEPATH += ../dependencies/boost
win32:CONFIG(release, debug|release): LIBS += -L../dependencies/boost/stage/lib/win32
else:win32:CONFIG(debug, debug|release): LIBS += -L../dependencies/boost/stage/lib/win32
win32:CONFIG(release, debug|release): LIBS += -lboost_date_time-mgw49-mt-1_57 -lboost_system-mgw49-mt-1_57 -lboost_thread-mgw49-mt-1_57 -lboost_chrono-mgw49-mt-1_57 -lws2_32
else:win32:CONFIG(debug, debug|release): LIBS += -lboost_date_time-mgw49-mt-1_57 -lboost_system-mgw49-mt-1_57 -lboost_thread-mgw49-mt-1_57 -lboost_chrono-mgw49-mt-1_57 -lws2_32
else:macx: LIBS += -L../dependencies/boost/stage/lib/macx/ -lboost_system -lboost_date_time -lboost_thread -lboost_filesystem -lboost_chrono
else:unix: LIBS += -L../dependencies/boost/stage/lib/linux/ -lboost_system -lboost_date_time -lboost_thread -lboost_filesystem -lboost_chrono
