﻿using UnityEngine;

[CreateAssetMenu(menuName = "Effects/ColorSet")]
public class ColorSetEffect : Effect
{
    public Color color = Color.white;
    
    protected override void Action(FeatureController feature)
    {
        feature.SetColor(color);
    }
}
