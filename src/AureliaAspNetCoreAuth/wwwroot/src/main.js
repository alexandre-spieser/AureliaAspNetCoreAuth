export function configure(aurelia) {
    aurelia.use
      .standardConfiguration()
      .developmentLogging()
      .plugin('aurelia-animator-css')
      .plugin('aurelia-validation')
      .plugin('aurelia-validatejs')
      .feature('bootstrap-validation');

    aurelia.start().then(() => aurelia.setRoot());
}