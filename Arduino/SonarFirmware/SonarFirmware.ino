
// Sonar - sample code from morzel.net blog post 

#include <NewPing.h>
#include <Servo.h>  

const byte setupReadyLedPin = 8;
const byte triggerPin = 10;
const byte echoPin = 11;
const byte servoPin = 12;

const byte maxDistanceInCm = 100;

byte angle;
byte angleStep;
byte angleStepDelayInMs = 50;

NewPing sonar(triggerPin, echoPin, maxDistanceInCm); 
Servo servo; 

void setup() {  
    pinMode(setupReadyLedPin, OUTPUT);
    
    angle = 0;
    angleStep = 1;
    
    servo.attach(servoPin);   
    servo.write(angle); 
    
    Serial.begin(9600); // Open connection with PC
    
    digitalWrite(setupReadyLedPin, HIGH);
}

void loop() {      
    alterServoMoveDirection();    
    
    measureAndSendDistance();
    
    angle += angleStep;
    servo.write(angle); // Move servo
    
    delay(angleStepDelayInMs);   
}

void alterServoMoveDirection() {
    if (angle == 180) {
       angleStep = -1; 
    } else if (angle == 0) {
       angleStep = 1;
    }
}

void measureAndSendDistance() {
    byte distanceInCm = sonar.ping_cm(); // Use ultrasound to measure distance   
      
    byte sonarData[] = {255, angle, distanceInCm};
    Serial.write(sonarData, 3); // Send data to PC
}
