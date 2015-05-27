package com.example.boszk_000.zgadywanka;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.graphics.drawable.Drawable;
import android.os.Handler;
import android.os.Message;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.Gravity;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.Spinner;
import android.widget.TableLayout;
import android.widget.TableRow;
import android.widget.TextView;
import android.widget.Toast;
import android.widget.AdapterView.OnItemSelectedListener;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.Timer;
import java.util.TimerTask;


public class EkranGry extends ActionBarActivity {

    private Context context;
    private Drawable reverseCard;
    private List<Drawable> images;
    private int [] [] cards;
    private Card firstCard;
    private Card secondCard;
    private static int wiersze = 0;
    private static int kolumny = 0;
    private TableLayout mainTable;
    private ButtonListener buttonListener;

    private static Object lock = new Object();
    private UpdateCardsHandler handler;

    private int koniec=0;
    int turns;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        handler = new UpdateCardsHandler();
        loadImages();

        int x,y;

        x = getIntent().getExtras().getInt("kol");
        y = getIntent().getExtras().getInt("wie");

        koniec = y*2; //koniec gry, jeżeli koniec = 0

        setContentView(R.layout.activity_ekran_gry);

        reverseCard =  getResources().getDrawable(R.drawable.icon);

        buttonListener = new ButtonListener();

        mainTable = (TableLayout)findViewById(R.id.TableLayout03);


        context  = mainTable.getContext();



        nowaGra(x, y);
    }

    private void nowaGra(int k, int w) {
        wiersze = w;
        kolumny = k;

        cards = new int [kolumny] [wiersze];

        mainTable.removeView(findViewById(R.id.TableRow01));
        //mainTable.removeView(findViewById(R.id.TableRow04));

        TableRow tr = ((TableRow)findViewById(R.id.TableRow03));
        tr.removeAllViews();

        mainTable = new TableLayout(context);
        tr.addView(mainTable);

        for (int y = 0; y < wiersze; y++) {
            mainTable.addView(createRow(y));
        }

        firstCard=null;
        loadCards();

        turns=0;
        ((TextView)findViewById(R.id.tv1)).setText("Ruchy: "+turns);
    }

    private void loadCards(){
        try{
            int size = wiersze*kolumny;

            Log.i("loadCards()","size=" + size);

            ArrayList<Integer> list = new ArrayList<Integer>();

            for(int i=0;i<size;i++){
                list.add(new Integer(i));
            }


            Random r = new Random();

            for(int i=size-1;i>=0;i--){
                int t=0;

                if(i>0){
                    t = r.nextInt(i);
                }

                t=list.remove(t).intValue();
                cards[i%kolumny][i/kolumny]=t%(size/2);

                Log.i("loadCards()", "card["+(i%kolumny)+
                        "]["+(i/kolumny)+"]=" + cards[i%kolumny][i/kolumny]);
            }
        }
        catch (Exception e) {
            Log.e("loadCards()", e+"");
        }

    }

    private TableRow createRow(int y){
        TableRow row = new TableRow(context);
        row.setHorizontalGravity(Gravity.CENTER);

        for (int x = 0; x < kolumny; x++) {
            row.addView(createImageButton(x,y));
        }
        return row;
    }

    private View createImageButton(int x, int y){
        Button button = new Button(context);
        button.setBackgroundDrawable(reverseCard);
        button.setId(100*x+y);
        button.setOnClickListener(buttonListener);
        return button;
    }

    class ButtonListener implements View.OnClickListener {

        @Override
        public void onClick(View v) {

            synchronized (lock) {
                if(firstCard!=null && secondCard != null){
                    return;
                }
                int id = v.getId();
                int x = id/100;
                int y = id%100;
                turnCard((Button)v,x,y);
            }

        }

        private void turnCard(Button button,int x, int y) {
            button.setBackgroundDrawable(images.get(cards[x][y]));

            if(firstCard==null){
                firstCard = new Card(button,x,y);
            }
            else{

                if(firstCard.x == x && firstCard.y == y){
                    return; //wciśnięto tą samą kartę
                }

                secondCard = new Card(button,x,y);

                turns++;//dodanie do wyniku
                ((TextView)findViewById(R.id.tv1)).setText("Liczba prób: "+turns);

                TimerTask tt = new TimerTask() {

                    @Override
                    public void run() {
                        try{
                            synchronized (lock) {
                                handler.sendEmptyMessage(0);
                            }
                        }
                        catch (Exception e) {
                            Log.e("E1", e.getMessage());
                        }
                    }
                };

                Timer t = new Timer(false);
                t.schedule(tt, 1300);
            }


        }

    }

    private void loadImages() {
        images = new ArrayList<Drawable>();

        images.add(getResources().getDrawable(R.drawable.card1));
        images.add(getResources().getDrawable(R.drawable.card2));
        images.add(getResources().getDrawable(R.drawable.card3));
        images.add(getResources().getDrawable(R.drawable.card4));
        images.add(getResources().getDrawable(R.drawable.card5));
        images.add(getResources().getDrawable(R.drawable.card6));
        images.add(getResources().getDrawable(R.drawable.card7));
        images.add(getResources().getDrawable(R.drawable.card8));
        images.add(getResources().getDrawable(R.drawable.card9));
        images.add(getResources().getDrawable(R.drawable.card10));
        images.add(getResources().getDrawable(R.drawable.card11));
        images.add(getResources().getDrawable(R.drawable.card12));
        images.add(getResources().getDrawable(R.drawable.card13));
        images.add(getResources().getDrawable(R.drawable.card14));
        images.add(getResources().getDrawable(R.drawable.card15));
        images.add(getResources().getDrawable(R.drawable.card16));
        images.add(getResources().getDrawable(R.drawable.card17));
        images.add(getResources().getDrawable(R.drawable.card18));
    }

    class UpdateCardsHandler extends Handler {

        @Override
        public void handleMessage(Message msg) {
            synchronized (lock) {
                checkCards();
            }
        }
        public void checkCards(){
            if(cards[secondCard.x][secondCard.y] == cards[firstCard.x][firstCard.y]){
                firstCard.button.setVisibility(View.INVISIBLE);
                secondCard.button.setVisibility(View.INVISIBLE);
                koniec=koniec-1;
            }
            else {
                secondCard.button.setBackgroundDrawable(reverseCard);
                firstCard.button.setBackgroundDrawable(reverseCard);
            }

            if(koniec==0)
            {
                Toast.makeText(getApplicationContext(), "Wygrana!!!\nPowróć do menu.\nTwój wynik to: "+turns,
                        Toast.LENGTH_LONG).show();
            }



            firstCard=null;
            secondCard=null;
        }
    }
}
