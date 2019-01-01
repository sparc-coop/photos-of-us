var PhotosOfUs;
(function (PhotosOfUs) {
    angular.module('app').controller('RandomPhotoCtrl', ['$scope', 'RandomPhotoClient', function ($scope, RandomPhotoClient) {
            function getRandomInt(min, max) {
                return Math.floor(Math.random() * (max - min + 1) + min);
            }
            // let area = document.getElementsByClassName("main-landing")[0];
            $scope.getRandomPosition = function (element) {
                var x = 800 - element.clientHeight;
                var y = document.body.offsetWidth - element.clientWidth;
                // var randomX = Math.floor(Math.random() * x);
                // var randomY = Math.floor(Math.random() * y);
                var coords = [getRandomInt(0, x), getRandomInt(650, 900)];
                return coords;
            };
            // $scope.loadImages = (ph) => {
            //    var img = document.createElement('img');
            //    img.setAttribute("src", ph.Url);
            //    img.setAttribute("style", "position:absolute; border-radius: 20px; max-width: 400px; max-height: 400px; margin: 50px;");
            //    var xy = $scope.getRandomPosition(img);
            //    img.style.top = xy[0] + 'px';
            //    img.style.left = xy[1] + 'px';
            //    var element = document.getElementById("main-landing");
            //    element.appendChild(img);
            // };
            $scope.photoList = [];
            $scope.getRandomPhotos = function (numPhotos) {
                var counter = 0;
                var interval = setInterval(function () {
                    counter += 1;
                    if (counter === numPhotos) {
                        clearInterval(interval);
                    }
                    RandomPhotoClient.get().then(function (photo) { return $scope.photoList.push(photo); });
                }, 0);
            };
        }]);
})(PhotosOfUs || (PhotosOfUs = {}));
//# sourceMappingURL=RandomPhotoController.js.map