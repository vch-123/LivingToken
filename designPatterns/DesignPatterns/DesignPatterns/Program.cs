class Program
{
    static void Main()
    {
        var character = new Character();

        character.Update();
        character.HandleInput("walk");
        character.Update();

        character.HandleInput("run");
        character.Update();

        character.HandleInput("jump");
        character.Update();
        character.Update();
        character.Update();

        character.HandleInput("crouch");
        character.Update();

        character.HandleInput("rest");
        character.Update();
    }
}