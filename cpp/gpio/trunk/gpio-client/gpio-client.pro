TEMPLATE = app
CONFIG += console
CONFIG -= qt
CONFIG += rtti
CONFIG += exceptions
#CONFIG += static

#QMAKE_CXXFLAGS += -std=gnu++0x

SOURCES += main.cpp \
    src/serial_port_device.cpp

HEADERS += \
    include/gpio/serial_port_device.h \
    include/gpio/gpio_device.h

#INCLUDEPATH += $$PWD/dependencies/asio-1.5.3/include

#win32:CONFIG(release, debug|release): LIBS += -L$$PWD/dependencies/boost-1.51/lib/ -llibboost_date_time-vc100-mt-1_51
#else:win32:CONFIG(debug, debug|release): LIBS += -L$$PWD/dependencies/boost-1.51/lib/ -llibboost_date_time-vc100-mt-gd-1_51

win32:CONFIG(release, debug|release): LIBS += -L../dependencies/boost/stage/lib
else:win32:CONFIG(debug, debug|release): LIBS += -L../dependencies/boost/stage/lib
win32:CONFIG(release, debug|release): LIBS += -lboost_date_time-mgw44-mt-1_47 -lboost_system-mgw44-mt-1_47 -lboost_thread-mgw44-mt-1_47 -lboost_chrono-mgw44-mt-1_47 -lws2_32
else:win32:CONFIG(debug, debug|release): LIBS += -lboost_date_time-mgw44-mt-1_47 -lboost_system-mgw44-mt-1_47 -lboost_thread-mgw44-mt-1_47 -lboost_chrono-mgw44-mt-1_47 -lws2_32

INCLUDEPATH += ../dependencies/boost
#DEPENDPATH += $$PWD/dependencies/boost-1.51

#win32:CONFIG(release, debug|release): PRE_TARGETDEPS += $$PWD/dependencies/boost-1.51/lib/libboost_date_time-vc100-mt-1_51.lib
#else:win32:CONFIG(debug, debug|release): PRE_TARGETDEPS += $$PWD/dependencies/boost-1.51/lib/libboost_date_time-vc100-mt-gd-1_51.lib

#INCLUDEPATH += "C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Include"
#win32:LIBS += -L"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Lib"
#win32:LIBS += WS2_32.lib
