using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellItemScriptableObject : ScriptableObject
{
    [NonReorderable]
    public List<Spell> spell_item = new List<Spell>();
}
