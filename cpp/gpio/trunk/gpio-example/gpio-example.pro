TEMPLATE = app
CONFIG += console
CONFIG -= qt
CONFIG += rtti
CONFIG += exceptions
#CONFIG += shared

SOURCES += main.cpp

win32:CONFIG(release, debug|release): LIBS += -L../dependencies/boost/stage/lib
else:win32:CONFIG(debug, debug|release): LIBS += -L../dependencies/boost/stage/lib
win32:CONFIG(release, debug|release): LIBS += -lboost_date_time-mgw44-mt-1_47 -lboost_system-mgw44-mt-1_47 -lboost_thread-mgw44-mt-1_47 -lboost_chrono-mgw44-mt-1_47 -lws2_32
else:win32:CONFIG(debug, debug|release): LIBS += -lboost_date_time-mgw44-mt-1_47 -lboost_system-mgw44-mt-1_47 -lboost_thread-mgw44-mt-1_47 -lboost_chrono-mgw44-mt-1_47 -lws2_32
#win32:CONFIG(release, debug|release): LIBS += -lboost_date_time-mgw44-mt-s-1_47 -lboost_system-mgw44-mt-s-1_47 -lboost_thread-mgw44-mt-s-1_47 -lboost_chrono-mgw44-mt-s-1_47 -lws2_32
#else:win32:CONFIG(debug, debug|release): LIBS += -lboost_date_time-mgw44-mt-s-1_47 -lboost_system-mgw44-mt-s-1_47 -lboost_thread-mgw44-mt-s-1_47 -lboost_chrono-mgw44-mt-s-1_47 -lws2_32

win32:CONFIG(release, debug|release): LIBS += -L../build/release
else:win32:CONFIG(debug, debug|release): LIBS += -L../build/debug

win32:CONFIG(release, debug|release): LIBS += -lgpio-client
else:win32:CONFIG(debug, debug|release): LIBS += -lgpio-client

INCLUDEPATH += ../dependencies/boost
INCLUDEPATH += ../gpio-client/include
