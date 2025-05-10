using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5001);  
});

var app = builder.Build();

var animals = new List<Animal>
{
    new Animal { Id = 1, Name = "Lucky", Category = "Panda", Weight = 77.5, FurColor = "White" },
    new Animal { Id = 2, Name = "Puszek", Category = "Cat", Weight = 5.5, FurColor = "Gray" },
    new Animal { Id = 3, Name = "piesek", Category = "Dog", Weight = 8.0, FurColor = "Black" },
new Animal { Id = 4, Name = "Milo", Category = "Cat", Weight = 6.0, FurColor = "Black" },
new Animal { Id = 5, Name = "ALi", Category = "Aligator", Weight = 10000, FurColor = "Green" }

};

var visits = new List<Visit>
{
    new Visit { Id = 1, AnimalId = 1, VisitDate = DateTime.Now.AddDays(-1), Description = "Routine checkup", Price = 2000.0 },
    new Visit { Id = 2, AnimalId = 2, VisitDate = DateTime.Now.AddDays(-2), Description = "Vaccination", Price = 4000.0 },
    new Visit { Id = 3, AnimalId = 3, VisitDate = DateTime.Now.AddDays(-5), Description = "Routine checkup", Price = 1500.0 },
    new Visit { Id = 4, AnimalId = 4, VisitDate = DateTime.Now.AddDays(-10), Description = "Spaying", Price = 2500.0 },
    new Visit { Id = 5, AnimalId = 5, VisitDate = DateTime.Now.AddDays(-7), Description = "Vaccination", Price = 3000.0 }
};

app.MapGet("/animals", () => Results.Ok(animals));

app.MapGet("/animals/{id}", (int id) => 
{
    var animal = animals.FirstOrDefault(a => a.Id == id);
    return animal is not null ? Results.Ok(animal) : Results.NotFound();
});

app.MapPost("/animals", (Animal animal) =>
{
    animal.Id = animals.Max(a => a.Id) + 1;
    animals.Add(animal);
    return Results.Created($"/animals/{animal.Id}", animal);
});

app.MapPut("/animals/{id}", (int id, Animal updatedAnimal) =>
{
    var animal = animals.FirstOrDefault(a => a.Id == id);
    if (animal is null) return Results.NotFound();

    animal.Name = updatedAnimal.Name;
    animal.Category = updatedAnimal.Category;
    animal.Weight = updatedAnimal.Weight;
    animal.FurColor = updatedAnimal.FurColor;

    return Results.Ok(animal);
});

app.MapDelete("/animals/{id}", (int id) =>
{
    var animal = animals.FirstOrDefault(a => a.Id == id);
    if (animal is null) return Results.NotFound();

    animals.Remove(animal);
    return Results.NoContent();
});

app.MapGet("/animals/search/{name}", (string name) =>
{
    var result = animals.Where(a => a.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
    return Results.Ok(result);
});

app.MapGet("/animals/{id}/visits", (int id) =>
{
    var animalVisits = visits.Where(v => v.AnimalId == id).ToList();
    return animalVisits.Any() ? Results.Ok(animalVisits) : Results.NotFound();
});

app.MapPost("/visits", (Visit visit) =>
{
    visit.Id = visits.Max(v => v.Id) + 1;
    visits.Add(visit);
    return Results.Created($"/visits/{visit.Id}", visit);
});

app.Run();

public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public double Weight { get; set; }
    public string FurColor { get; set; }
}

public class Visit
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public DateTime VisitDate { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
}
