(function (global) {

    // map tells the System loader where to look for things
    var map = {
        'app': 'appScripts', // 'dist',
        'rxjs': 'https://npmcdn.com/rxjs@5.0.0-beta.12', // <- BAD, should be 'lib/rxjs' so we can support 100% offline mode, but stupid ng2 won't support single rxjs umd file at present, see http://stackoverflow.com/questions/38793349/angular2-systemjs-failed-to-use-rx-umd-js
        'angular2-in-memory-web-api': 'lib/angular2-in-memory-web-api',
        '@angular': 'lib/@angular'
    };

    // packages tells the System loader how to load when no filename and/or no extension
    var packages = {
        'app': { main: 'boot.js', defaultExtension: 'js' },
        'rxjs': { main: 'bundles/Rx.umd.js', defaultExtension: 'js' },
        'angular2-in-memory-web-api': { defaultExtension: 'js' }
    };

    var packageNames = [
      'common',
      'compiler',
      'core',
      'forms',
      'http',
      'platform-browser',
      'platform-browser-dynamic',
      'router',
      'testing',
      'upgrade'
    ];

    // add package entries for angular packages in the form '@angular/common': { main: 'index.js', defaultExtension: 'js' }
    packageNames.forEach(function (pkgName) {
        packages['@angular/' + pkgName] = { main: 'bundles/' + pkgName + '.umd.js', defaultExtension: 'js' };
    });

    var config = {
        map: map,
        packages: packages
    }

    // filterSystemConfig - index.html's chance to modify config before we register it.
    if (global.filterSystemConfig) { global.filterSystemConfig(config); }

    System.config(config);

})(this);