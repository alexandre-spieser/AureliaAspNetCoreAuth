import {BootstrapFormValidationRenderer} from './bootstrap-form-validation-renderer';

export function configure(config) {
    config.container.registerHandler(
      'bootstrap-form',
      container => container.get(BootstrapFormValidationRenderer));
}
