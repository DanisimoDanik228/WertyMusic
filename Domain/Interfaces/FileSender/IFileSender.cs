using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Domain.Interfaces;

public interface IFileSender
{
    Task<Stream?> GetFileAsync(Music music);
}