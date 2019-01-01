namespace PhotosOfUs {
    angular.module('app').controller('RandomPhotoCtrl',
        ['$scope', 'RandomPhotoClient', ($scope, RandomPhotoClient: RandomPhotoClient) => {

        function getRandomInt(min: number, max: number) {
            return Math.floor(Math.random() * (max - min + 1) + min);
        }

        // let area = document.getElementsByClassName("main-landing")[0];
        $scope.getRandomPosition = () => {
            //console.log(element);
            //const x = 800 - element.clientHeight;
            //const y = document.body.offsetWidth - element.clientWidth;
            // var randomX = Math.floor(Math.random() * x);
            // var randomY = Math.floor(Math.random() * y);
            return [getRandomInt(0, 80) + '%', getRandomInt(0, 80) + '%'];
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

        $scope.getRandomPhotos = (numPhotos: number) => {
            let counter = 0;
            const interval = setInterval(function () {
                counter += 1;
                if (counter >= numPhotos) {
                    clearInterval(interval);
                }

                RandomPhotoClient.get().then(photo => {
                    photo['coords'] = $scope.getRandomPosition();
                    $scope.photoList.push(photo);
                });
            }, 0);
        };
    }]);
}
