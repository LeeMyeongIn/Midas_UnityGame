using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/CookRecipe")]
public class CookRecipe : ScriptableObject
{
    public int recipeId;
    public string recipeName;
    public Sprite recipeIcon;
    public List<RecipeIngredient> ingredients;
    public Item resultItem;
}

