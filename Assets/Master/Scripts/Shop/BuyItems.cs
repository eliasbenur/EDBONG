using UnityEngine;

public class BuyItems : MonoBehaviour
{
    private bool contact;
    private Player_Movement player_Controller;
    public GameObject stuff;
    private GameManager players_stats;

    private void Awake()
    {
        player_Controller = GetComponent<Player_Movement>();
        players_stats = Camera.main.GetComponent<GameManager>();
    }

    private void Update()
    {
        if(contact && stuff!= null)
        {
            if(player_Controller.rew_player.GetButton("Items"))
            {
                var stuff_Buy = stuff.GetComponent<StuffDisplay>().stuff;
                if (players_stats.money >= stuff_Buy.cost)
                {
                    //Health Bonus
                    if ((players_stats.life += stuff_Buy.life) >= players_stats.max_Life)
                    {
                        players_stats.life = players_stats.max_Life;
                        //Update of the UI
                        players_stats.Update_liveDisplay();
                    }                      
                    else
                    {
                        players_stats.life += stuff_Buy.life;
                        players_stats.Update_liveDisplay();
                    }                        
                    //Speed Bonus
                    if (stuff_Buy.speedBoost != 0)
                    {
                        for (int i = 0; i < players_stats.players_Movement.Count; i++)
                        {
                            players_stats.players_Movement[i].speed = stuff_Buy.speedBoost;
                        }
                    }
                    //No shield value max here
                    //Shield Bonus
                    players_stats.shieldPoint += stuff_Buy.shield;
                    players_stats.Update_shieldDisyplay();

                    //Play sound for buy here
                    AkSoundEngine.PostEvent("play_buy_item", Camera.main.gameObject);
                    Destroy(stuff);
                }
                
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Stuff")
        {
            contact = true;
            stuff = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag =="Stuff")
        {
            contact = false;
            stuff = null;
        }
    }
}
