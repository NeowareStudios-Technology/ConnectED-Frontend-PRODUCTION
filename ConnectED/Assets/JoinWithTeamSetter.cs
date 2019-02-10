using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class JoinWithTeamSetter : MonoBehaviour {


    public RawImage pic;
    public Text teamName;
    private Team t;
    //this initializes all the teams in the join with team popup
    public void setTeam(Team team)
    {
        t = new Team();
        t = team;
        Texture2D tex = new Texture2D(400, 400);
        if (team.t_photo != null && team.t_photo.Length > 300)
        {
            byte[] img = System.Convert.FromBase64String(team.t_photo);
            tex.LoadImage(img, false);

            pic.texture = tex;
        }
        teamName.text = team.t_name;
    }
}
