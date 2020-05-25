package com.example.projekt;

import android.app.Application;
import android.location.Location;

public class MyApp extends Application {
    Location location;
    boolean isSetLocation = false;

    public MyApp() {
    }

    public Location getLocation() {
        return location;
    }

    public void setLocation(Location location) {
        this.location = location;
        isSetLocation = true;
    }
}
