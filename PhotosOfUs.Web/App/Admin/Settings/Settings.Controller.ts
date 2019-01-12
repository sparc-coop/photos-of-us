namespace PhotosOfUs {
    angular.module('app').controller('SettingsCtrl', ['$scope', 'Upload', '$timeout', ($scope, Upload, $timeout) => {
        $scope.settings = {
            homepageTemplate: 2
        };

        $scope.$watch('settings.featuredImage', () => {
            if ($scope.settings.featuredImage && !$scope.settings.featuredImage.error) {
                Upload.upload({
                    url: '/api/Admin/Photos',
                    data: {
                      file: $scope.settings.featuredImage
                    }
                }).then(resp => {
                    $scope.settings.featuredImageUrl = resp.data.Url;
                }, null, evt => {
                    const progressPercentage = 100.0 * evt.loaded / evt.total;
                    console.log('progress: ' + progressPercentage +
                      '% ' + evt.config.data.file.name);
                });
            }
        });

        $scope.removeFeaturedImage = ($event) => {
            $scope.settings.featuredImageUrl = null;
            $scope.settings.featuredImage = null;
            $event.stopPropagation();
        };
    }]);
}
