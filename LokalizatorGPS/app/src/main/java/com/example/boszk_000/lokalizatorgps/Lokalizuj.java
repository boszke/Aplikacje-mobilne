package com.example.boszk_000.lokalizatorgps;

import android.app.AlertDialog;
import android.app.Service;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.media.audiofx.BassBoost;
import android.os.Bundle;
import android.os.IBinder;
import android.provider.Settings;

/**
 * Created by boszk_000 on 2015-05-06.
 */
public class Lokalizuj extends Service implements LocationListener {

    private final Context context;

    boolean czyGPSDziala = false;
    boolean czySiecDziala = false;
    boolean dasieustaliclokacje = false;

    Location location;

    double wysokosc;
    double szerokosc;

    private static final long MIN_DISTANCE_CHANGE_FOR_UPDATES = 10;
    private static final long MIN_TIME_BW_UPDATES = 1000 * 60 * 1;

    protected LocationManager locationManager;

    public Lokalizuj(Context context)
    {
        this.context = context;
        getLocation();
    }

    public Location getLocation() {
        try
        {
            locationManager = (LocationManager) context.getSystemService(LOCATION_SERVICE);

            czyGPSDziala = locationManager.isProviderEnabled(LocationManager.GPS_PROVIDER);

            czySiecDziala = locationManager.isProviderEnabled(LocationManager.NETWORK_PROVIDER);

            if(!czyGPSDziala && !czySiecDziala)
            {

            }
            else
            {
                this.dasieustaliclokacje = true;

                if (czySiecDziala)
                {
                    locationManager.requestLocationUpdates(LocationManager.NETWORK_PROVIDER, MIN_TIME_BW_UPDATES, MIN_DISTANCE_CHANGE_FOR_UPDATES, this);

                    if (locationManager != null)
                    {
                        location = locationManager.getLastKnownLocation(LocationManager.NETWORK_PROVIDER);

                        if (location != null)
                        {
                            szerokosc = location.getLatitude();
                            wysokosc = location.getLongitude();
                        }
                    }
                }

                if(czyGPSDziala)
                {
                    if(location == null)
                    {
                        locationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER,MIN_TIME_BW_UPDATES,MIN_DISTANCE_CHANGE_FOR_UPDATES,this);

                        if (locationManager != null)
                        {
                            location = locationManager.getLastKnownLocation(locationManager.GPS_PROVIDER);

                            if (location != null)
                            {
                                szerokosc = location.getLatitude();
                                wysokosc = location.getLongitude();
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e){
            e.printStackTrace();
        }

        return location;
    }

    public void stopUsingGPS()
    {
        if (locationManager != null)
        {
            locationManager.removeUpdates(Lokalizuj.this);
        }
    }

    public double getLatitude()
    {
        if(location != null)
        {
            wysokosc = location.getLongitude();
        }
        return wysokosc;
    }

    public double getLongitude()
    {
        if(location != null)
        {
            szerokosc = location.getLatitude();
        }
        return szerokosc;
    }

    public boolean dasieustaliclokacje()
    {
        return this.dasieustaliclokacje;
    }

    public void pokazBledy()
    {
        AlertDialog.Builder alertDialog = new AlertDialog.Builder(context);

        alertDialog.setTitle("Ustawienia GPS");

        alertDialog.setMessage("GPS nie jest włączony. Chcesz przejść do ustawień?");

        alertDialog.setPositiveButton("Ustawienia",new DialogInterface.OnClickListener()
        {
            @Override
            public void onClick(DialogInterface dialog, int which)
            {
                Intent intent = new Intent(Settings.ACTION_LOCATION_SOURCE_SETTINGS);
                context.startActivity(intent);
            }
        });

        alertDialog.setNegativeButton("Anuluj", new DialogInterface.OnClickListener()
        {
            @Override
            public void onClick(DialogInterface dialog, int which)
            {
                dialog.cancel();
            }
        });

        alertDialog.show();
    }

    @Override
    public void onLocationChanged(Location location) {

    }

    @Override
    public void onStatusChanged(String provider, int status, Bundle extras) {

    }

    @Override
    public void onProviderEnabled(String provider) {

    }

    @Override
    public void onProviderDisabled(String provider) {

    }

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }
}
