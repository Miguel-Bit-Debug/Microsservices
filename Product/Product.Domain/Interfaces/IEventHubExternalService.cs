﻿using Product.Domain.DTOs.Response;

namespace Product.Domain.Interfaces
{
    public interface IEventHubExternalService<T> where T : class
    {
        Task<GenericResponse> PublishAsync(string user, T message, string token);
    }
}
