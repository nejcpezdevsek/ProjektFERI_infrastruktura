package com.example.projekt;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.provider.MediaStore;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;

public class MainActivity extends AppCompatActivity implements SensorEventListener {

    final static int REQUEST_CODE = 0;
    ImageView imageV;

    private SensorManager sensorManager;
    Sensor accelerometer;
    Sensor gyroscope;

    TextView gyroscopeTV;
    TextView accelerometerTV;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        Button CameraButton = findViewById(R.id.cameraButton);

        CameraButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent i = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
                startActivityForResult(i, REQUEST_CODE);
            }
        });

        gyroscopeTV = findViewById(R.id.gyroscopeTV);
        accelerometerTV = findViewById(R.id.accelerometerTV);

        sensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);
        gyroscope = sensorManager.getDefaultSensor(Sensor.TYPE_GYROSCOPE);
        sensorManager.registerListener(MainActivity.this, gyroscope, SensorManager.SENSOR_DELAY_NORMAL);

        sensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);
        accelerometer = sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
        sensorManager.registerListener(MainActivity.this, accelerometer, SensorManager.SENSOR_DELAY_NORMAL);
    }

    protected void onActivityResult(int requestCode, int resultCode, Intent intent) {

        super.onActivityResult(requestCode, resultCode, intent);
        if(resultCode == RESULT_OK){
            Bundle extras = intent.getExtras();
            Bitmap data = (Bitmap) extras.get("data");
            imageV = (ImageView) findViewById(R.id.capturedImage);
            imageV.setImageBitmap(data);
        }
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int i){

    }


    @Override
    public void onSensorChanged(SensorEvent sensorEvent){
        if(sensorEvent.sensor.getType() == Sensor.TYPE_GYROSCOPE){
            String reading = "X: " + sensorEvent.values[0] + "\n" + "Y: " + sensorEvent.values[1] + "\n" + "Z: " + sensorEvent.values[2];
            gyroscopeTV.setText(reading);
        }
        if(sensorEvent.sensor.getType() == Sensor.TYPE_ACCELEROMETER){
            String reading = "X: " + sensorEvent.values[0] + "\n" + "Y: " + sensorEvent.values[1] + "\n" + "Z: " + sensorEvent.values[2];
            accelerometerTV.setText(reading);
        }
    }
}
