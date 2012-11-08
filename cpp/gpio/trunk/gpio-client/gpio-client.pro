TEMPLATE = lib
CONFIG += console
CONFIG -= qt
CONFIG += rtti
CONFIG += exceptions
CONFIG += dll

SOURCES += \
    src/serial_port_device.cpp

HEADERS += \
    include/gpio/serial_port_device.h \
    include/gpio/gpio_device.h

win32:CONFIG(release, debug|release): LIBS += -L../dependencies/boost/stage/lib
else:win32:CONFIG(debug, debug|release): LIBS += -L../dependencies/boost/stage/lib
win32:CONFIG(release, debug|release): LIBS += -lboost_date_time-mgw44-mt-1_47 -lboost_system-mgw44-mt-1_47 -lboost_thread-mgw44-mt-1_47 -lboost_chrono-mgw44-mt-1_47 -lws2_32
else:win32:CONFIG(debug, debug|release): LIBS += -lboost_date_time-mgw44-mt-1_47 -lboost_system-mgw44-mt-1_47 -lboost_thread-mgw44-mt-1_47 -lboost_chrono-mgw44-mt-1_47 -lws2_32

INCLUDEPATH += include
INCLUDEPATH += ../dependencies/boost
