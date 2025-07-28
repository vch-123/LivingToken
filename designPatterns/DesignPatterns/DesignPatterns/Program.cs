var original = new Enemy
{
    Name = "Orc",
    Weapon = new Weapon { Name = "Axe" }
};

var clone = original.Clone();

clone.Name = "Troll";
clone.Weapon.Name = "Sword";

Console.WriteLine(original.Name);  // Orc
Console.WriteLine(original.Weapon.Name); // Sword ❗（浅拷贝问题）
