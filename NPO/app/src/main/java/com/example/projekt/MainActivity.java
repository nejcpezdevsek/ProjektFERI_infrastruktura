package com.example.projekt;

import androidx.annotation.RequiresApi;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.FileProvider;

import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.ImageDecoder;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.provider.SyncStateContract;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import net.gotev.uploadservice.MultipartUploadRequest;
import net.gotev.uploadservice.UploadNotificationConfig;

import org.apache.http.NameValuePair;
import org.apache.http.message.BasicNameValuePair;

import java.io.BufferedWriter;
import java.io.File;
import java.io.IOException;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.URL;
import java.nio.charset.StandardCharsets;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.UUID;

import javax.net.ssl.HttpsURLConnection;

public class MainActivity extends AppCompatActivity implements SensorEventListener {

    final static int REQUEST_IMAGE_CAPTURE = 1;
    ImageView imageV;

    String currentPhotoPath = "";

    private SensorManager sensorManager;
    Sensor accelerometer;
    Sensor gyroscope;

    TextView gyroscopeTV;
    TextView accelerometerTV;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

          /*if(Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            CharSequence name = "Upload";
            String description = "Uploading image notification.";
            int importance = NotificationManager.IMPORTANCE_DEFAULT;
            NotificationChannel channel = new NotificationChannel("1", name, importance);
            channel.setDescription(description);
            // Register the channel with the system; you can't change the importance
            // or other notification behaviors after this
            NotificationManager notificationManager = getSystemService(NotificationManager.class);
            assert notificationManager != null;
            notificationManager.createNotificationChannel(channel);
        }*/

        Button GPSButton = findViewById(R.id.GPSButton);
        Button CameraButton = findViewById(R.id.cameraButton);
        Button SendData = findViewById(R.id.sendData);

        imageV = findViewById(R.id.capturedImage);

        GPSButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent i = new Intent(getBaseContext(), GpsActivity.class);
                startActivity(i);
            }
        });

        CameraButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent i = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
                File photoFile = null;
                try {
                    photoFile = createImageFile();
                } catch (IOException e) {
                    e.printStackTrace();
                }

                if (photoFile != null) {
                    Uri photoURI = FileProvider.getUriForFile(getApplicationContext(), "com.example.projekt.fileprovider", photoFile);
                    i.putExtra(MediaStore.EXTRA_OUTPUT, photoURI);
                    startActivityForResult(i, REQUEST_IMAGE_CAPTURE);
                }
            }
        });

        SendData.setOnClickListener(new View.OnClickListener(){
            @Override
            public void onClick(View v) {
                if(!currentPhotoPath.equals("")){
                    Thread thread = new Thread(new Runnable() {
                        @Override
                        public void run() {
                            uploadMultipart();
                        }
                    });
                    thread.start();
                }else{
                    Toast.makeText(getApplicationContext(), "Nothing to upload!", Toast.LENGTH_SHORT).show();
                }
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

    @RequiresApi(api = Build.VERSION_CODES.P)
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if(resultCode != RESULT_CANCELED){
            if (requestCode == REQUEST_IMAGE_CAPTURE && resultCode == RESULT_OK) {
                try {
                    setPic();
                } catch (Exception e) {
                    e.printStackTrace();
                }
            }
        }
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int i){

    }


    @Override
    public void onSensorChanged(SensorEvent sensorEvent){
        if(sensorEvent.sensor.getType() == Sensor.TYPE_GYROSCOPE){
            String reading = "\nX: " + sensorEvent.values[0] + "\n" + "Y: " + sensorEvent.values[1] + "\n" + "Z: " + sensorEvent.values[2];
            gyroscopeTV.setText(reading);
        }
        if(sensorEvent.sensor.getType() == Sensor.TYPE_ACCELEROMETER){
            String reading = "\nX: " + sensorEvent.values[0] + "\n" + "Y: " + sensorEvent.values[1] + "\n" + "Z: " + sensorEvent.values[2];
            accelerometerTV.setText(reading);
        }
    }

    private File createImageFile() throws IOException {

        String timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").format(new Date());
        String imageFileName = "JPEG_" + timeStamp + "_";
        File storageDir = getExternalFilesDir(Environment.DIRECTORY_PICTURES);
        File image = File.createTempFile(
                imageFileName,  /* prefix */
                ".jpg",         /* suffix */
                storageDir      /* directory */
        );
        currentPhotoPath = image.getAbsolutePath();
        return image;
    }

    private void setPic() {
        BitmapFactory.Options bmOptions = new BitmapFactory.Options();
        bmOptions.inJustDecodeBounds = true;
        int scaleFactor = 3;

        bmOptions.inJustDecodeBounds = false;
        bmOptions.inSampleSize = scaleFactor;
        bmOptions.inPurgeable = true;

        Bitmap bitmap = BitmapFactory.decodeFile(currentPhotoPath, bmOptions);
        imageV.setImageBitmap(bitmap);
    }

    public void uploadMultipart() {
        try {
            URL url = new URL("http://192.168.64.100:3000/phonedata/");
            HttpURLConnection conn = (HttpURLConnection) url.openConnection();
            conn.setReadTimeout(10000);
            conn.setConnectTimeout(15000);
            conn.setRequestMethod("POST");
            conn.setDoInput(true);
            conn.setDoOutput(true);

            List<NameValuePair> params = new ArrayList<>();
            params.add(new BasicNameValuePair("gyroscope", gyroscopeTV.getText().toString()));
            params.add(new BasicNameValuePair("accelerometer", accelerometerTV.getText().toString()));
            params.add(new BasicNameValuePair("image", currentPhotoPath));

            OutputStream os = conn.getOutputStream();
            BufferedWriter writer = new BufferedWriter(
                    new OutputStreamWriter(os, StandardCharsets.UTF_8));
            writer.write(String.valueOf(params));
            writer.flush();
            writer.close();
            os.close();

            conn.connect();
            runOnUiThread(() -> Toast.makeText(getApplicationContext(), "Upload Successful!", Toast.LENGTH_SHORT).show());
        } catch (Exception exc) {
            Toast.makeText(this, exc.getMessage(), Toast.LENGTH_SHORT).show();
        }
    }
}
