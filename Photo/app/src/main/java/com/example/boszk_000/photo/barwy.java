package com.example.boszk_000.photo;

import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.graphics.drawable.BitmapDrawable;
import android.net.Uri;
import android.os.Environment;
import android.provider.MediaStore;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ImageView;
import android.widget.SeekBar;
import android.widget.Toast;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStream;


public class barwy extends ActionBarActivity {


    private ImageView image;
    private Bitmap bmp;
    private Bitmap operation;
    private SeekBar seekBarR, seekBarG, seekBarB;
    private int seekR=0, seekG=0, seekB=0;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_barwy);

        image = (ImageView)findViewById(R.id.imageView2);
        Bundle bundle = this.getIntent().getExtras();
        Bitmap bm = bundle.getParcelable("BITMAP");
        image.setImageBitmap(bm);

        BitmapDrawable abmp = (BitmapDrawable)image.getDrawable();
        bmp = abmp.getBitmap();

        //suwak Red
        seekBarR =(SeekBar)findViewById(R.id.seekBarR);

        seekBarR.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {

            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                seekR = progress;
                ChangeColors();
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {

            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {

            }
        });

        //suwak Green
        seekBarG =(SeekBar)findViewById(R.id.seekBarG);

        seekBarG.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {

            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                seekG = progress;
                ChangeColors();
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {

            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {

            }
        });

        //suwak Blue
        seekBarB =(SeekBar)findViewById(R.id.seekBarB);

        seekBarB.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {

            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                seekB = progress;
                ChangeColors();
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {

            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {

            }
        });
    }

    private void ChangeColors() {
        int color = Color.rgb(seekR, seekG, seekB);
        // Zmiana na suwakach
        operation = Bitmap.createBitmap(bmp.getWidth(),
                bmp.getHeight(),bmp.getConfig());

        for(int i=0; i<bmp.getWidth();i++)
        {
            for(int j=0;j<bmp.getHeight();j++)
            {
                int p = bmp.getPixel(i,j);
                int r = Color.red(p) + seekR;
                int g = Color.green(p)+seekG;
                int b = Color.blue(p)+seekB;

                operation.setPixel(i, j, Color.argb(Color.alpha(p),r, g, b));

            }
        }
        image.setImageBitmap(operation);
    }

    public void gray(View view)
    {
        operation = Bitmap.createBitmap(bmp.getWidth(),
                bmp.getHeight(),bmp.getConfig());

        double red = 0.33;
        double green = 0.59;
        double blue = 0.11;

        for(int i=0; i<bmp.getWidth();i++)
        {
            for(int j=0;j<bmp.getHeight();j++)
            {
                int p = bmp.getPixel(i,j);

                int r = Color.red(p);
                int g = Color.green(p);
                int b = Color.blue(p);

                r=g=b=(int)(red*r+green*g+blue*b);

                operation.setPixel(i, j, Color.argb(Color.alpha(p), r, g, b));
            }
        }
        image.setImageBitmap(operation);
    }

    public void oryginal(View view)
    {
        image.setImageBitmap(bmp);
        seekBarR.setProgress(0);
        seekBarG.setProgress(0);
        seekBarB.setProgress(0);
    }

    public void negatyw(View view)
    {
        operation = Bitmap.createBitmap(bmp.getWidth(),
                bmp.getHeight(),bmp.getConfig());

        for(int i=0; i<bmp.getWidth();i++)
        {
            for(int j=0;j<bmp.getHeight();j++)
            {
                int p = bmp.getPixel(i,j);
                int r = Color.red(p);
                int g = Color.green(p);
                int b = Color.blue(p);

                operation.setPixel(i, j, Color.argb(Color.alpha(p),255-r, 255-g, 255-b));
            }
        }
        image.setImageBitmap(operation);
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_barwy, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    public void zapisz(View view){
        image.buildDrawingCache();
        Bitmap bm=image.getDrawingCache();

        OutputStream fOut = null;
        Uri outputFileUri;
        try {
            File root = new File(Environment.getExternalStorageDirectory()
                    + File.separator + "DCIM/Camera" + File.separator);
            root.mkdirs();
            File sdImageMainDirectory = new File(root, "mojaFotka.jpg");
            outputFileUri = Uri.fromFile(sdImageMainDirectory);
            fOut = new FileOutputStream(sdImageMainDirectory);
            Toast.makeText(this, "Zdjęcie zapisano w: "+ root,
                    Toast.LENGTH_SHORT).show();
        } catch (Exception e) {
            Toast.makeText(this, "Błąd zapisu",
                    Toast.LENGTH_SHORT).show();
        }

        try {
            bm.compress(Bitmap.CompressFormat.PNG, 100, fOut);
            fOut.flush();
            fOut.close();
        } catch (Exception e) {
        }
        }


}
