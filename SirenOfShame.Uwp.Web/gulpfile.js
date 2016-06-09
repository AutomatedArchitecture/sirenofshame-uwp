/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    ts = require('gulp-typescript');

var htmlreplace = require('gulp-html-replace');

var paths = {
    webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("scriptsNStyles", function() {
    gulp.src([
            'es6-shim/es6-shim.min.js',
            'systemjs/dist/system-polyfills.js',
            'systemjs/dist/system.src.js',
            'reflect-metadata/Reflect.js',
            'rxjs/**',
            'zone.js/dist/**',
            '@angular/**'
    ], {
        cwd: "node_modules/**"
    })
    .pipe(gulp.dest("./wwwroot/lib"));
});

gulp.task("picopy", function() {
    var dst = '../SirenOfShame.Uwp.Background/wwwroot/';
    console.log("Destination: " + dst);
    return gulp.src([
        paths.webroot + 'index.html',
        paths.webroot + 'systemjs.config.js',
        paths.webroot + 'css/*',
        paths.webroot + 'images/*',
        paths.webroot + 'appScripts/*',
        paths.webroot + 'lib/bootstrap/dist/css/bootstrap.min.css',
        paths.webroot + 'lib/bootstrap/dist/js/bootstrap.min.js',
        paths.webroot + 'lib/jquery/dist/jquery.min.js',
        paths.webroot + 'lib/es6-shim/es6-shim.min.js',
        paths.webroot + 'lib/zone.js/dist/zone.js',
        paths.webroot + 'lib/reflect-metadata/Reflect.js',
        paths.webroot + 'lib/systemjs/dist/system.src.js',
        paths.webroot + 'systemjs.config.js',
        paths.webroot + 'lib/rxjs/**/*.js',
        paths.webroot + 'lib/@angular/**/*.js'
    ], { base: 'wwwroot' })
        .pipe(gulp.dest(dst));
});

gulp.task("min", ["min:js", "min:css"]);

gulp.task("pideploy", ["picopy"]);

var tsProject = ts.createProject('Ts/tsconfig.json',
{
    typescript: require('typescript'),
    outFile: '../wwwroot/appScripts/app.js',
    inlineSourceMap: true,
    declaration: false,
    stripInternal: true,
    module: 'system',
    noEmitOnError: false,
    rootDir: '.',
    inlineSources: true
});
gulp.task('ts', function (done) {
    //var tsResult = tsProject.src()
    var tsResult = gulp.src([
            "Ts/*.ts"
    ])
        .pipe(ts(tsProject), undefined, ts.reporter.fullReporter());

    return tsResult.js
        .pipe(concat('app.min.js'))
        //.pipe(uglify())
        .pipe(gulp.dest('./wwwroot/dist'));
});

gulp.task('watch', ['watch.ts']);

gulp.task('watch.ts', ['ts'], function () {
    return gulp.watch('scripts/*.ts', ['ts']);
});

gulp.task('vendor-bundle', function () {
    gulp.src([
            'es6-shim/es6-shim.min.js',
            'systemjs/dist/system-polyfills.js',
            'zone.js/dist/zone.js',
            'systemjs/dist/system.src.js',
            'rxjs/bundles/Rx.js',
            'reflect-metadata/Reflect.js'
    ], {
        cwd: "node_modules/**"
    })
    .pipe(concat('vendors.min.js'))
    .pipe(uglify())
    .pipe(gulp.dest('./wwwroot/dist'));
});

gulp.task('ng2-bundle', function() {
    gulp.src([
      'node_modules/@angular/common/common.umd.js',
      'node_modules/@angular/compiler/compiler.umd.js',
      'node_modules/@angular/core/core.umd.js',
      'node_modules/@angular/http/http.umd.js',
      'node_modules/@angular/platform-browser/platform-browser.umd.js',
      'node_modules/@angular/platform-browser-dynamic/platform-browser-dynamic.umd.js',
      'node_modules/@angular/router/router.umd.js'
    ], { base: 'node_modules' })
    .pipe(gulp.dest('./wwwroot/dist'));
});

gulp.task('boot-bundle', function () {
    gulp.src('./wwwroot/config.prod.js')
      .pipe(concat('boot.min.js'))
      .pipe(uglify())
      .pipe(gulp.dest('./wwwroot/dist'));
});

gulp.task('systemconfig-bundle', function () {
    gulp.src('./wwwroot/systemjs.config.prod.js')
      .pipe(concat('systemjs.config.js'))
      .pipe(uglify())
      .pipe(gulp.dest('./wwwroot/dist'));
});

gulp.task('html', function () {
    gulp.src('./wwwroot/index.html')
      .pipe(htmlreplace({
          'vendor': 'vendors.min.js',
          'app': 'app.min.js',
          'boot': 'boot.min.js'
      }))
      .pipe(gulp.dest('./wwwroot/dist'));
});

gulp.task('default', ['scriptsNStyles', 'watch']);