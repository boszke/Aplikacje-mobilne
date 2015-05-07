package com.example.boszk_000.lokalizatorgps;

import android.content.Intent;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;


public class MainActivity extends ActionBarActivity
{


    Button btnPokazLokacje;

    Lokalizuj gps;

    public void sendMessage(View view)
    {
        Intent intent = new Intent(this, Wyswietlanie.class);
        EditText editText = (EditText) findViewById(R.id.edit_message);
        String message = editText.getText().toString();
        intent.putExtra("EXTRA_MESSAGE", message);
        startActivity(intent);
    }

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        btnPokazLokacje = (Button) findViewById(R.id.Pokaz_lokalizacje);

        btnPokazLokacje.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View v)
            {
                gps = new Lokalizuj(MainActivity.this);

                if(gps.dasieustaliclokacje())
                {
                    double szerokosc = gps.getLatitude();
                    double wysokosc = gps.getLongitude();

                    Toast.makeText(getApplicationContext(),"Twoje położenie: \nSzerokość: " +szerokosc + "\nWysokość: " + wysokosc, Toast.LENGTH_LONG).show();
                }
                else
                {
                    gps.pokazBledy();
                }
            }
        });
    }
}
