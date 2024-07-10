using System.ComponentModel;
using CSharpLocalAndRemote.Cache;
using CSharpLocalAndRemote.model;

namespace Tests.Cache;

public class CacheGenericTest
{
    private Tenista CreateRandomTenista()
    {
        var random = new Random();
        return new Tenista(
            id: random.NextInt64(1, 1000),
            nombre: "Tenista",
            pais: "Pais",
            altura: 180,
            peso: 75,
            puntos: 1000,
            mano: Mano.Diestro,
            fechaNacimiento: new DateTime(1990, 1, 1)
        );
    }

    [Test]
    [DisplayName("Debe almacenar un elemento en la caché")]
    public void DebeDevolverUnElementoExistenteEnBaseSuClave()
    {
        var cache = new CacheGeneric<long, Tenista>(2);
        var tenista1 = CreateRandomTenista();

        cache.Put(tenista1.Id, tenista1);

        Assert.Multiple(() =>
        {
            Assert.NotNull(cache.Get(tenista1.Id), "El tenista debería estar presente en la caché");
            Assert.That(cache.Get(tenista1.Id), Is.EqualTo(tenista1),
                "El tenista recuperado debería ser igual al tenista almacenado");
        });
    }

    [Test]
    [DisplayName("Debe devolver null si el elemento no existe en la caché")]
    public void DebeDevolverNullSiElementoNoExisteEnBaseSuClave()
    {
        var cache = new CacheGeneric<long, Tenista>(2);
        var nonExistingId = new Random().NextInt64();

        Assert.IsNull(cache.Get(nonExistingId), "Debería devolver null si el tenista no existe en la caché");
    }

    [Test]
    [DisplayName("Debe actualizar un elemento en la caché")]
    public void DebeIntroducirElementosEnTodaCache()
    {
        var cache = new CacheGeneric<long, Tenista>(3);
        var tenista1 = CreateRandomTenista();
        var tenista2 = CreateRandomTenista();

        cache.Put(tenista1.Id, tenista1);
        cache.Put(tenista2.Id, tenista2);

        Assert.Multiple(() =>
        {
            Assert.That(cache.Size(), Is.EqualTo(2), "La caché debería contener dos elementos");
            Assert.That(cache.Get(tenista1.Id), Is.EqualTo(tenista1),
                "El tenista1 recuperado debería ser igual al tenista1 almacenado");
            Assert.That(cache.Get(tenista2.Id), Is.EqualTo(tenista2),
                "El tenista2 recuperado debería ser igual al tenista2 almacenado");
        });
    }

    [Test]
    [DisplayName("Debe eliminar un elemento de la caché")]
    public void DebeEliminarUnTenistaSiLimiteSuperaAlIntroducir()
    {
        var cache = new CacheGeneric<long, Tenista>(2);
        var tenista1 = CreateRandomTenista();
        var tenista2 = CreateRandomTenista();
        var tenista3 = CreateRandomTenista();
        var tenista4 = CreateRandomTenista();

        cache.Put(tenista1.Id, tenista1);
        cache.Put(tenista2.Id, tenista2);
        cache.Put(tenista1.Id, tenista1);
        cache.Put(tenista3.Id, tenista3);
        cache.Put(tenista4.Id, tenista4);
        cache.Put(tenista1.Id, tenista1);

        Assert.Multiple(() =>
        {
            Assert.That(cache.Size(), Is.EqualTo(2), "La caché debería tener dos elementos");
            Assert.IsFalse(cache.ContainsKey(tenista2.Id), "La caché no debería contener tenista2");
            Assert.IsFalse(cache.ContainsKey(tenista3.Id), "La caché no debería contener tenista3");
            Assert.That(cache.Get(tenista1.Id), Is.EqualTo(tenista1), "El tenista1 debería estar aún en la caché");
            Assert.That(cache.Get(tenista4.Id), Is.EqualTo(tenista4), "El tenista4 debería estar en la caché");
        });
    }

    [Test]
    [DisplayName("Debe eliminar los elementos de la caché")]
    public void DebeEliminarLosElementosDeLaCache()
    {
        var cache = new CacheGeneric<long, Tenista>(2);
        var tenista1 = CreateRandomTenista();
        var tenista2 = CreateRandomTenista();

        cache.Put(tenista1.Id, tenista1);
        cache.Put(tenista2.Id, tenista2);

        cache.Clear();

        Assert.Multiple(() =>
        {
            Assert.That(cache.Size(), Is.EqualTo(0), "La caché debería estar vacía");
            Assert.IsFalse(cache.ContainsKey(tenista1.Id), "La caché no debería contener tenista1");
            Assert.IsFalse(cache.ContainsKey(tenista2.Id), "La caché no debería contener tenista2");
        });
    }

    [Test]
    [DisplayName("Debe devolver claves y valores correctos")]
    public void DebeDevolverClavesYValoresCorrectos()
    {
        var cache = new CacheGeneric<long, Tenista>(2);
        var tenista1 = CreateRandomTenista();
        var tenista2 = CreateRandomTenista();

        cache.Put(tenista1.Id, tenista1);
        cache.Put(tenista2.Id, tenista2);

        Assert.Multiple(() =>
        {
            CollectionAssert.AreEquivalent(new HashSet<long> { tenista1.Id, tenista2.Id }, cache.Keys(),
                "Las claves devueltas deberían coincidir con las esperadas");
            CollectionAssert.AreEquivalent(new List<Tenista> { tenista1, tenista2 }, cache.Values(),
                "Los valores devueltos deberían coincidir con los esperados");
        });
    }

    [Test]
    [DisplayName("Debe comprobar existencia de clave en la caché")]
    public void DebeComprobarExistenciaDeValorEnBaseSuClave()
    {
        var cache = new CacheGeneric<long, Tenista>(2);
        var tenista1 = CreateRandomTenista();

        cache.Put(tenista1.Id, tenista1);

        Assert.Multiple(() =>
        {
            Assert.IsTrue(cache.ContainsKey(tenista1.Id), "La caché debería contener el tenista1 por su clave");
            Assert.IsFalse(cache.ContainsKey(-99L),
                "La caché no debería contener un tenista con una clave inexistente");
        });
    }

    [Test]
    [DisplayName("Debe comprobar si contiene un valor")]
    public void DebeComprobarSiExisteUnValor()
    {
        var cache = new CacheGeneric<long, Tenista>(2);
        var tenista1 = CreateRandomTenista();
        var tenista2 = CreateRandomTenista();

        cache.Put(tenista1.Id, tenista1);

        Assert.Multiple(() =>
        {
            Assert.IsTrue(cache.ContainsValue(tenista1), "é debería contener tenista1");
            Assert.IsFalse(cache.ContainsValue(tenista2), "La caché no debería contener tenista2");
        });
    }

    [Test]
    [DisplayName("Debe comprobar si está vacía")]
    public void DebeComprobarSiEstaVacia()
    {
        var cache = new CacheGeneric<long, Tenista>(2);

        Assert.Multiple(() =>
        {
            Assert.IsTrue(cache.IsEmpty(), "La caché debería estar vacía");
            Assert.IsFalse(cache.IsEmpty() == false, "La caché no debería estar no-vacía");
        });

        var tenista1 = CreateRandomTenista();
        cache.Put(tenista1.Id, tenista1);

        Assert.Multiple(() =>
        {
            Assert.IsFalse(cache.IsEmpty(), "La caché no debería estar vacía");
            Assert.IsTrue(cache.IsEmpty() == false, "La caché debería estar no-vacía");
        });
    }
}