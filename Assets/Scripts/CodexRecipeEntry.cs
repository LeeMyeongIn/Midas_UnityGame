using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodexRecipeEntry : MonoBehaviour
{
    public Image recipeIcon;

    public void Initialize(CookRecipe recipe, bool isUnlocked, bool isCooked)
    {
        recipeIcon.sprite = recipe.recipeIcon;

        if (!isUnlocked)
        {
            recipeIcon.color = Color.black;
        }
        else
        {
            bool hasCooked = RecipeUnlockManager.Instance.IsCooked(recipe.recipeId);

            if (hasCooked)
            {
                recipeIcon.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                recipeIcon.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
        }
    }
}
