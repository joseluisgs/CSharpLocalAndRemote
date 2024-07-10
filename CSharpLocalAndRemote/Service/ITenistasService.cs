using CSharpFunctionalExtensions;
using CSharpLocalAndRemote.Error;
using CSharpLocalAndRemote.model;

namespace CSharpLocalAndRemote.Service;

/**
 * Esta vez en vez de Task, usamos IObservable para poder hacer uso de Rx
 * Esta vez solo lo hago para que practiques, podrías seguir igual que antes con tus tasks
 * Pero si no lo haces no vas a aprender!!!
 */
public interface ITenistasService
{
    const long RefreshTime = 50000;

    IObservable<Result<List<Tenista>, TenistaError>> GetAll(bool fromRemote);

    IObservable<Result<Tenista, TenistaError>> GetById(long id);

    IObservable<Result<Tenista, TenistaError>> Save(Tenista tenista);

    IObservable<Result<Tenista, TenistaError>> Update(long id, Tenista tenista);

    IObservable<Result<long, TenistaError>> Delete(long id);

    IObservable<Result<int, TenistaError>> ImportData(FileInfo file);

    IObservable<Result<int, TenistaError>> ExportData(FileInfo file, bool fromRemote);

    void Refresh();

    void LoadData();
}