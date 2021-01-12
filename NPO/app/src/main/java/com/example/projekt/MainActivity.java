package com.example.projekt;

import androidx.annotation.RequiresApi;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.FileProvider;

import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.location.Location;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.os.VibrationEffect;
import android.os.Vibrator;
import android.provider.MediaStore;
import android.util.Base64;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;
import android.Manifest;

import com.google.android.gms.location.FusedLocationProviderClient;
import com.google.android.gms.location.LocationServices;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.tasks.OnSuccessListener;
import com.google.android.gms.tasks.Task;

import org.apache.http.NameValuePair;
import org.apache.http.message.BasicNameValuePair;

import java.io.BufferedWriter;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.IOException;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.UnsupportedEncodingException;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

public class MainActivity<ArrayListList> extends AppCompatActivity implements SensorEventListener {

    final static int REQUEST_IMAGE_CAPTURE = 1;
    ImageView imageV;

    String currentPhotoPath = "";

    private SensorManager sensorManager;
    Vibrator v;
    Sensor accelerometer;
    Sensor gyroscope;

    TextView gyroscopeTV;
    TextView accelerometerTV;

    MyApp App;

    //*SPREMENLJIVKE ZA ALGORITEM PREPOZNAVE SLABE CESTE*//
    double X, Y, Z;
    private double mAccel;
    private double mAccelCurrent;
    private double mAccelLast;

    // GPS SPREMENLJIVKE
    private static final int REQUEST_CODE = 101;
    FusedLocationProviderClient fusedLocationProviderClient;
    Location currentLocation;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        App = (MyApp) getApplication();
        v = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
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
                if(!currentPhotoPath.equals("") && App.isSetLocation){
                    Thread thread = new Thread(new Runnable() {
                        @Override
                        public void run() {
                            uploadMultipart();
                        }
                    });
                    thread.start();
                }else{
                    runOnUiThread(() ->Toast.makeText(getApplicationContext(), "Nothing to upload!", Toast.LENGTH_SHORT).show());
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

        fusedLocationProviderClient = LocationServices.getFusedLocationProviderClient(this);

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

    private int compare(int X, int Y, int Z) {
        X = Math.abs(X);
        Y = Math.abs(Y);
        Z = Math.abs(Z);
        if (X > Y) {
            if (X > Z) return 0;
        } else if (Y > Z) return 1;
        else return 2;

        return -1;
    }

    private void bumpDetected() {
        if(ActivityCompat.checkSelfPermission(this, Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED){
            ActivityCompat.requestPermissions(this, new String[]
                    {Manifest.permission.ACCESS_FINE_LOCATION}, REQUEST_CODE);
            return;
        }
        Task<Location> task = fusedLocationProviderClient.getLastLocation();
        task.addOnSuccessListener(location -> {
            if(location != null){
                currentLocation = location;
            App.addBump(new LatLng(currentLocation.getLatitude(), currentLocation.getLongitude()));
            //Toast.makeText(getApplicationContext(),bumps.size(),Toast.LENGTH_SHORT).show();
            }
        });
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

            //dobim vrednosti X,Y,Z od pospeškometra
            X = sensorEvent.values[0];
            Y = sensorEvent.values[1];
            Z = sensorEvent.values[2];
            //trenutno vrednost pospeškometra shranim v mAccelLast
            mAccelLast = mAccelCurrent;
            //izračunam trenutno vrednost pospeškometra
            mAccelCurrent = Math.sqrt(Math.pow(X, 2) + Math.pow(Y, 2) + Math.pow(Z, 2));
            //izračunam razliko med izračunanim pospeškom in prejšnjim pospeškom, ki sem ga shranil
            double delta = mAccelCurrent - mAccelLast;
            //izračunam pospešek
            mAccel = mAccel * 0.9f + delta;
            //dobim, katera koordinata je, ali X, Y, Z
            int temp = compare((int) X, (int) Y, (int) Z);
            if (temp == 0) {
                if ((mAccelLast - mAccelCurrent) > 3) {
                    Toast.makeText(this, "Luknja na X", Toast.LENGTH_SHORT).show();
                    //v.vibrate(VibrationEffect.createOneShot(500, VibrationEffect.DEFAULT_AMPLITUDE));
                    bumpDetected();
                }
            } else if (temp == 1) {
                if ((mAccelLast - mAccelCurrent) > 3) {
                    Toast.makeText(this, "Luknja na Y", Toast.LENGTH_SHORT).show();
                    //v.vibrate(VibrationEffect.createOneShot(500, VibrationEffect.DEFAULT_AMPLITUDE));
                    bumpDetected();
                }
            } else if (temp == 2) {
                if ((mAccelLast - mAccelCurrent) > 3) {
                    Toast.makeText(this, "Luknja na Z", Toast.LENGTH_SHORT).show();
                    //v.vibrate(VibrationEffect.createOneShot(500, VibrationEffect.DEFAULT_AMPLITUDE));
                    bumpDetected();
                }
            }
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
            URL url = new URL("http://192.168.2.28:3000/phonedata");
            HttpURLConnection conn = (HttpURLConnection) url.openConnection();
            conn.setReadTimeout(10000);
            conn.setConnectTimeout(15000);
            conn.setRequestMethod("POST");
            conn.setDoInput(true);
            conn.setDoOutput(true);

            Bitmap bitmap = BitmapFactory.decodeFile(currentPhotoPath);
            Bitmap resizedBitmap = Bitmap.createScaledBitmap(bitmap, (int)(bitmap.getWidth()*0.2), (int)(bitmap.getHeight()*0.2), true);

            ByteArrayOutputStream stream = new ByteArrayOutputStream();

            resizedBitmap.compress(Bitmap.CompressFormat.JPEG, 50, stream);


            byte[] byteArray= stream.toByteArray();
            String imageString= Base64.encodeToString(byteArray, Base64.DEFAULT);

            List<NameValuePair> params = new ArrayList<>();
            params.add(new BasicNameValuePair("phone_name", Build.MODEL));
            params.add(new BasicNameValuePair("gyroscope", gyroscopeTV.getText().toString()));
            params.add(new BasicNameValuePair("accelerometer", accelerometerTV.getText().toString()));
            params.add(new BasicNameValuePair("image", imageString));
            params.add(new BasicNameValuePair("latitude", App.getLocation().getLatitude()+""));
            params.add(new BasicNameValuePair("longitude", App.getLocation().getLongitude()+""));

            OutputStream os = conn.getOutputStream();
            BufferedWriter writer = new BufferedWriter(
                    new OutputStreamWriter(os, StandardCharsets.UTF_8));
            writer.write(getQuery(params));
            writer.flush();
            writer.close();
            os.close();

            int responseCode=conn.getResponseCode();

            conn.connect();
            if(responseCode==201){
                runOnUiThread(() -> Toast.makeText(getApplicationContext(), "Upload Successful!", Toast.LENGTH_SHORT).show());
            }else{
                runOnUiThread(() -> Toast.makeText(getApplicationContext(), "Error! Response code: "+responseCode, Toast.LENGTH_SHORT).show());
            }
        } catch (Exception exc) {
            runOnUiThread(() ->Toast.makeText(this, exc.getMessage(), Toast.LENGTH_SHORT).show());
        }
    }

    private String getQuery(List<NameValuePair> params) throws UnsupportedEncodingException
    {
        StringBuilder result = new StringBuilder();
        boolean first = true;

        for (NameValuePair pair : params)
        {
            if (first)
                first = false;
            else
                result.append("&");

            result.append(URLEncoder.encode(pair.getName(), "UTF-8"));
            result.append("=");
            result.append(URLEncoder.encode(pair.getValue(), "UTF-8"));
        }

        return result.toString();
    }
}
