using System;
using ClinicAppointmentReservation.Domain.Enums;
using ClinicAppointmentReservation.Domain.Records;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointmentReservation.WebAPI.Extentions;

public static class ControllerResultExtension
{
    public static IActionResult ToProblem(this ControllerBase controller, Error error) => error.Type switch
    {
        ErrorType.Conflict => controller.Problem(statusCode: 409, title: error.Message),
        ErrorType.NotFound => controller.Problem(statusCode: 404, title: error.Message),
        ErrorType.Forbidden => controller.Problem(statusCode: 403, title: error.Message),
        _ => controller.Problem()  
    };
    public static IActionResult ToCreatedAtActionResult<T>(this ControllerBase controller, Result<T> result, string actionName, object routeValues)
    {
        if (result.IsSuccess)
        {
            return controller.CreatedAtAction(actionName, routeValues, result.Content!.Value);
        }
        else
        {
            return ToProblem(controller, result.Error!);
        }
    }
    public static IActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.IsSuccess)
        {
            switch (result.Content!.Type)
            {
                case SuccessType.Ok:
                    return controller.Ok(result.Content.Value);
                case SuccessType.NoContent:
                    return controller.NoContent();
                default:
                    return controller.Ok();
            }
        }
        else
        {
            return ToProblem(controller, result.Error!);
        }
    }
}
