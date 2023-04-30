using System;
using System.Collections.Generic;

namespace SpiceTeamJamming.UI
{
    public static class UIRouter
    {
        public enum RouteType
        {
            Main,
            Settings,
            SettingsControls,
            SettingsAudio,
            SettingsGraphics,
            SettingsAccessibility,
            Pause,
            LevelSelection,
            Tutorial,
            Map,
            Battlefield
        }

        private static readonly Dictionary<RouteType, UIView> _routeToViewBridge = new();
        internal static UILoadingScreen LoadingScreen;

        private static readonly Stack<RouteType> _routesHistoric = new ();
        private static RouteType _currentRoute = RouteType.Main;

        public static RouteType CurrentRoute => _currentRoute;

        public static event Action<RouteType, RouteType> RouteChangeEvent; 

        public static void ConfigureRoute(RouteType route, UIView view)
        {
            if (_routeToViewBridge.ContainsKey(route)) return;
        
            _routeToViewBridge.Add(route, view);
        }

        public static void ClearRoute(RouteType route)
        {
            if (!_routeToViewBridge.ContainsKey(route)) return;

            _routeToViewBridge.Remove(route);
        }

        public static void GoToRoute(RouteType route, bool saveToHistoric = true)
        {
            if (!_routeToViewBridge.ContainsKey(route)) return;

            var leavingRoute = _currentRoute;
            var enteringRoute = route;
            
            var leavingView = _routeToViewBridge[_currentRoute];
            var enteringView = _routeToViewBridge[route];
        
            leavingView.Hide();
            enteringView.Show();
            
            if (saveToHistoric)
                _routesHistoric.Push(_currentRoute);
            
            _currentRoute = route;
            
            RouteChangeEvent?.Invoke(leavingRoute, enteringRoute);
        }

        public static void GoToPreviousRoute()
        {
            if (_routesHistoric.Count == 0) return;
        
            var previousRoute = _routesHistoric.Pop();
            GoToRoute(previousRoute, false);
        }

        public static void ShowLoadingScreen()
        {
            LoadingScreen.Show();
        }

        public static void HideLoadingScreen()
        {
            LoadingScreen.Hide();
        }
    }
}
