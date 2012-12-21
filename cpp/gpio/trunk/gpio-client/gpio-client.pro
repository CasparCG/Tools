TARGET = gpio-client
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

win32:CONFIG(release, debug|release): LIBS += -L../dependencies/boost/stage/lib/win32
else:win32:CONFIG(debug, debug|release): LIBS += -L../dependencies/boost/stage/lib/win32
win32:CONFIG(release, debug|release): LIBS += -lboost_date_time-mgw44-mt-1_47 -lboost_system-mgw44-mt-1_47 -lboost_thread-mgw44-mt-1_47 -lboost_chrono-mgw44-mt-1_47 -lws2_32
else:win32:CONFIG(debug, debug|release): LIBS += -lboost_date_time-mgw44-mt-1_47 -lboost_system-mgw44-mt-1_47 -lboost_thread-mgw44-mt-1_47 -lboost_chrono-mgw44-mt-1_47 -lws2_32
else:macx: LIBS += -L$$PWD/../../dependencies/boost/stage/lib/macx/ -lboost_date_time-mt -lboost_system-mt -lboost_thread-mt -lboost_filesystem-mt -lboost_chrono-mt

INCLUDEPATH += include
INCLUDEPATH += ../dependencies/boost
