#ifndef GPIO_LOG_H_
#define GPIO_LOG_H_

#include <Arduino.h>

#define ENABLE_LOGGING

#ifdef ENABLE_LOGGING
  #define log(message) \
    Serial.print(__FILE__); \
    Serial.print(":"); \
    Serial.print(__LINE__); \
    Serial.print(": "); \
    Serial.println(message);
#else
  #define log(message)
#endif

#endif

