package com.example.boszk_000.zgadywanka;

import android.content.Intent;
import android.graphics.drawable.Drawable;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;

import java.util.ArrayList;
import java.util.List;


public class MainActivity extends ActionBarActivity {


    public static int wie=0;
    public static int kol=0;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    }

    public void playGame4x4(View view)
    {
        Intent intent=new Intent(this, EkranGry.class);
        int wie=4;
        int kol=4;
        intent.putExtra("wie", wie);
        intent.putExtra("kol", kol);
        startActivity(intent);
    }

    public void playGame5x5(View view)
    {
        Intent intent=new Intent(this, EkranGry.class);
        int wie=5;
        int kol=4;
        intent.putExtra("wie", wie);
        intent.putExtra("kol", kol);
        startActivity(intent);
    }

    public void playGame6x6(View view)
    {
        Intent intent=new Intent(this, EkranGry.class);
        int wie=6;
        int kol=4;
        intent.putExtra("wie", wie);
        intent.putExtra("kol", kol);
        startActivity(intent);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_main, menu);
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
}
