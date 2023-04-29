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

        private static Stack<RouteType> _routesHistory = new ();
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

        public static void GoToRoute(RouteType route)
        {
            if (!_routeToViewBridge.ContainsKey(route)) return;
            
            var leavingView = _routeToViewBridge[_currentRoute];
            var enteringView = _routeToViewBridge[route];
        
            leavingView.Hide();
            enteringView.Show();
            
            _routesHistory.Push(_currentRoute);
            _currentRoute = route;
            
            RouteChangeEvent?.Invoke(_routesHistory.Peek(), _currentRoute);
        }

        public static void GoToPreviousRoute()
        {
            if (_routesHistory.Count == 0) return;
        
            var previousRoute = _routesHistory.Pop();
            GoToRoute(previousRoute);
        }
    }
}
