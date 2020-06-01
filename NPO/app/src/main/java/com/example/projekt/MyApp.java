package com.example.projekt;

import android.app.Application;
import android.location.Location;

import com.google.android.gms.maps.model.LatLng;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;

public class MyApp extends Application {
    Location location;
    boolean isSetLocation = false;
    Map<LatLng,String> bumps = new HashMap<LatLng,String>();

    public MyApp() {
    }
    public Map<LatLng,String> getBumps(){ return bumps;}
    public Location getLocation() {
        return location;
    }

    public void setLocation(Location location) {
        this.location = location;
        isSetLocation = true;
    }
    public void addBump(LatLng bump){
        SimpleDateFormat formatter = new SimpleDateFormat("dd/MM/yyyy HH:mm:ss");
        Date date = new Date();
        bumps.put(bump,formatter.format(date));
    }
}
