// See https://aka.ms/new-console-template for more information

using CSharpLocalAndRemote.Mapper;
using CSharpLocalAndRemote.model;

Console.WriteLine("🎾🎾 Hola Tenistas! 🎾🎾");

var tenista = new Tenista(
    nombre: "Rafael Nadal",
    pais: "España",
    altura: 185,
    peso: 85,
    puntos: 10250,
    mano: Mano.DIESTRO,
    fechaNacimiento: new DateTime(1986, 6, 3),
    id: 1
);

Console.WriteLine($"Tenista: {tenista}");

var tenistaDto = tenista.ToTenistaDto();
Console.WriteLine($"TenistaDto: {tenistaDto}");

var tenista2 = tenistaDto.ToTenista();
Console.WriteLine($"Tenista2: {tenista2}");