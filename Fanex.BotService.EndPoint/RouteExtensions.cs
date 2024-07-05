namespace Fanex.BotService.EndPoint
{
    public static class RouteExtensions
    {
        public static string PathBase { get; set; }

        public static string GetActualRouteValue(string routeValue)
        => string.IsNullOrWhiteSpace(PathBase) ? routeValue : PathBase + "/" + routeValue;
    }
}