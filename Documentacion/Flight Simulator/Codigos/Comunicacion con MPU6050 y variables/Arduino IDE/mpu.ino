#include <Wire.h>
#include <Adafruit_MPU6050.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_Sensor.h>

Adafruit_MPU6050 mpu;

void setup() {
  Serial.begin(115200);
  while (!Serial) {
    delay(10); // Espera hasta que el serial esté listo
  }

  // Inicializa el MPU6050
  if (!mpu.begin()) {
    Serial.println("No se encontro el mpu");
    while (1) {
      delay(10);
    }
  }

  Serial.println("MPU6050 encontrado!");
  mpu.setAccelerometerRange(MPU6050_RANGE_8_G);
  mpu.setGyroRange(MPU6050_RANGE_500_DEG);
  mpu.setFilterBandwidth(MPU6050_BAND_21_HZ);
}

void loop() {
  /* Get new sensor events with the readings */
  sensors_event_t a, g, temp;
  mpu.getEvent(&a, &g, &temp);

  /* Print out the values */
  Serial.print("AccelX:");
  Serial.print(a.acceleration.x);
  Serial.print(", AccelY:");
  Serial.print(a.acceleration.y);
  
  Serial.print(", GyroX:");
  Serial.print(g.gyro.x);
  Serial.print(", GyroY:");
  Serial.print(g.gyro.y);

  delay(500); // Espera 500ms antes de la siguiente lectura
}
