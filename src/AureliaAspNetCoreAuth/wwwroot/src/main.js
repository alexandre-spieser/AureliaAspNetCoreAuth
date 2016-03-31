export function configure(aurelia) {
    aurelia.use
      .standardConfiguration()
      .developmentLogging()
      .plugin('aurelia-animator-css')
      .plugin("aurelia-validation");

    aurelia.start().then(() => aurelia.setRoot());
}