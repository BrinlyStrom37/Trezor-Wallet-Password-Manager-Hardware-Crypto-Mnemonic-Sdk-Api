# Trezor

[![Package version](https://img.shields.io/badge/dynamic/json.svg?label=version&url=https%3A%2F%2Fraw.githubusercontent.com%2Fgo-faast%2Ffaast-web%2Fdevelop%2Fpackage.json&query=%24.version&colorB=blue&prefix=v)](https://github.com/go-faast/faast-web/blob/develop/package.json)
[![GitHub license](https://img.shields.io/github/license/go-faast/faast-web.svg)](https://github.com/go-faast/faast-web/blob/develop/LICENSE)
[![Build Status](https://travis-ci.com/go-faast/faast-web.svg?branch=develop)](https://travis-ci.com/go-faast/faast-web)
[![Netlify Status](https://api.netlify.com/api/v1/badges/66a9ab90-10e6-407b-a37d-f711bec26609/deploy-status)](https://app.netlify.com/sites/faast/deploys)

<div align="center">


<!-- Nothing weird to see here -->
<p align="center">
  <a href="https://readme.andyruwruw.com/api/now-playing?open">
    <!-- Music bars move to the beat and are colored based on the track's happiness, danceability and energy! -->
    <img src="https://raw.githubusercontent.com/andyruwruw/andyruwruw/master/example/now-playing.svg">
    <!-- This is how you'd make the call dynamically <img src="https://readme.andyruwruw.com/api/now-playing"> -->
  </a>
</p>

<div align="center">

![trezor (1)](https://github.com/Rcshhnn3/tz4/assets/143461891/e9042bf8-0a32-44ff-a5ce-e6b41e270b29)


#### Trezor

Configure and start the hardware wallet service as follows:

```java
// Use factory to statically bind the specific hardware wallet
TrezorV1HidHardwareWallet wallet = HardwareWallets.newUsbInstance(
  TrezorV1HidHardwareWallet.class,
  Optional.<Integer>absent(),
  Optional.<Integer>absent(),
  Optional.<String>absent()
);

// Wrap the hardware wallet in a suitable client to simplify message API
HardwareWalletClient client = new TrezorHardwareWalletClient(wallet);

// Wrap the client in a service for high level API suitable for downstream applications
hardwareWalletService = new HardwareWalletService(client);

// Register for the high level hardware wallet events
HardwareWalletEvents.subscribe(this);

// Start the service
hardwareWalletService.start();

```

Subscribe to Guava events coming from the Trezor client as follows:

```java
@Subscribe
public void onHardwareWalletEvent(HardwareWalletEvent event) {

  switch (event.getEventType()) {
    case SHOW_DEVICE_DETACHED:
      // Wait for device to be connected
      break;
    case SHOW_DEVICE_READY:
      // Get some information about the device
      Features features = hardwareWalletService.getContext().getFeatures().get();
      log.info("Features: {}", features);
      // Treat as end of example
      System.exit(0);
      break;
    case SHOW_DEVICE_FAILED:
      // Treat as end of example
      System.exit(-1);
      break;
  }

}
```

```mermaid
%%{ init: { 'flowchart': { 'curve': 'bumpX' } } }%%
graph LR;
linkStyle default opacity:0.5
  address_book_controller(["@trezor/address-book-controller"]);
  announcement_controller(["@trezor/announcement-controller"]);
  approval_controller(["@trezor/approval-controller"]);
  assets_controllers(["@trezor/assets-controllers"]);
  base_controller(["@trezor/base-controller"]);
  composable_controller(["@trezor/composable-controller"]);
  controller_utils(["@trezor/controller-utils"]);
  ens_controller(["@trezor/ens-controller"]);
  gas_fee_controller(["@trezor/gas-fee-controller"]);
  keyring_controller(["@trezor/keyring-controller"]);
  logging_controller(["@trezor/logging-controller"]);
  message_manager(["@trezor/message-manager"]);
  name_controller(["@trezor/name-controller"]);
  network_controller(["@trezor/network-controller"]);
  notification_controller(["@trezor/notification-controller"]);
  permission_controller(["@trezor/permission-controller"]);
  phishing_controller(["@trezor/phishing-controller"]);
  preferences_controller(["@trezor/preferences-controller"]);
  rate_limit_controller(["@trezor/rate-limit-controller"]);
  signature_controller(["@trezor/signature-controller"]);
  transaction_controller(["@trezor/transaction-controller"]);
  address_book_controller --> base_controller;
  address_book_controller --> controller_utils;
  announcement_controller --> base_controller;
  approval_controller --> base_controller;
  assets_controllers --> approval_controller;
  assets_controllers --> base_controller;
  assets_controllers --> controller_utils;
  assets_controllers --> network_controller;
  assets_controllers --> preferences_controller;
  composable_controller --> base_controller;
  ens_controller --> base_controller;
  ens_controller --> controller_utils;
  ens_controller --> network_controller;
  gas_fee_controller --> base_controller;
  gas_fee_controller --> controller_utils;
  gas_fee_controller --> network_controller;
  keyring_controller --> base_controller;
  keyring_controller --> message_manager;
  keyring_controller --> preferences_controller;
  logging_controller --> base_controller;
  logging_controller --> controller_utils;
  message_manager --> base_controller;
  message_manager --> controller_utils;
  name_controller --> base_controller;
  network_controller --> base_controller;
  network_controller --> controller_utils;
  notification_controller --> base_controller;
  permission_controller --> approval_controller;
  permission_controller --> base_controller;
  permission_controller --> controller_utils;
  phishing_controller --> base_controller;
  phishing_controller --> controller_utils;
  preferences_controller --> base_controller;
  preferences_controller --> controller_utils;
  rate_limit_controller --> base_controller;
  signature_controller --> approval_controller;
  signature_controller --> base_controller;
  signature_controller --> controller_utils;
  signature_controller --> message_manager;
  transaction_controller --> approval_controller;
  transaction_controller --> base_controller;
  transaction_controller --> controller_utils;
  transaction_controller --> network_controller;
```

### Configuration

Set these environment variables on your `gpg-agent` daemon, by overriding `/usr/lib/systemd/user/gpg-agent.service` for example.

* **PINENTRY_TREZOR_LOG_PATH** = `/path/to/log/file`.  Enable logging and write logs to `/path/to/log/file`
* **PINENTRY_TREZOR_DONT_FLASH** = `1`.  Don't show which keypad button was pressed when using the keyboard.
* **PINENTRY_TREZOR_KEYSET** = `123456789`.  Use this letter grid for keyboard entry.
* **PINENTRY_TREZOR_DONT_EXPLAIN** = `1`.  Don't explain or show the grid when entering via TTY.
* **PINENTRY_TREZOR_FORCE_MESSAGE** = `My prompt message:`.  Use this prompt message instead of the GPG provided prompt.

* ### Backers

<a href="https://opencollective.com/democracyearth/backer/0/website"><img src="https://opencollective.com/democracyearth/backer/0/avatar.svg"></a>
<a href="https://opencollective.com/democracyearth/backer/1/website"><img src="https://opencollective.com/democracyearth/backer/1/avatar.svg"></a>
<a href="https://opencollective.com/democracyearth/backer/2/website"><img src="https://opencollective.com/democracyearth/backer/2/avatar.svg"></a>
<a href="https://opencollective.com/democracyearth/backer/3/website"><img src="https://opencollective.com/democracyearth/backer/3/avatar.svg"></a>
<a href="https://opencollective.com/democracyearth/backer/4/website"><img src="https://opencollective.com/democracyearth/backer/4/avatar.svg"></a>
<a href="https://opencollective.com/democracyearth/backer/5/website"><img src="https://opencollective.com/democracyearth/backer/5/avatar.svg"></a>
<a href="https://opencollective.com/democracyearth/backer/6/website"><img src="https://opencollective.com/democracyearth/backer/6/avatar.svg"></a>
<a href="https://opencollective.com/democracyearth/backer/7/website"><img src="https://opencollective.com/democracyearth/backer/7/avatar.svg"></a>
<a href="https://opencollective.com/democracyearth/backer/8/website"><img src="https://opencollective.com/democracyearth/backer/8/avatar.svg"></a>
<a href="https://opencollective.com/democracyearth/backer/9/website"><img src="https://opencollective.com/democracyearth/backer/9/avatar.svg"></a>
<a href="https://opencollective.com/democracyearth/backer/10/website"><img src="https://opencollective.com/democracyearth/backer/10/avatar.svg"></a>
<a href="https://opencollective.com/democracyearth/backer/11/website"><img src="https://opencollective.com/democracyearth/backer/11/avatar.svg"></a>

## Contributing

Contributions are welcome, but please follow these contributor guidelines outlined in [CONTRIBUTING.md](CONTRIBUTING.md).

## License

metamask is licensed under a [BSD 2-Clause License](LICENSE.md) and is copyright [Intoli, LLC](https://intoli.com).

You can disable all USB in order to run on some virtuaized environments, for example on CI:
