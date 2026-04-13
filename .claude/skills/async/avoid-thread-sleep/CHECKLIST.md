# Avoid Thread.Sleep Checklist

- [ ] `Thread.Sleep(...)` is not used as the default waiting mechanism
- [ ] `Task.Delay(...)` is preferred in async code
- [ ] Waiting is necessary and intentional
- [ ] Blocking waits are avoided in server, UI, and worker paths
- [ ] Any true thread-blocking case is documented clearly
