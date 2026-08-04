<template>
  <div class="app-shell">
    <header class="app-header">
      <div class="app-header-inner">
        <router-link to="/" class="brand">
          <span class="brand-name">Meeting Room Bookings</span>
        </router-link>
        <nav class="app-nav">
          <router-link to="/">Home</router-link>
        </nav>
      </div>
    </header>

    <main class="app-main">
      <router-view />
    </main>
  </div>
</template>

<style scoped>
.app-shell {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.app-header {
  position: sticky;
  top: 0;
  z-index: 20;
  background: rgba(10, 14, 22, 0.85);
  backdrop-filter: blur(10px);
  border-bottom: 1px solid var(--c-border);
}

.app-header-inner {
  max-width: 1080px;
  margin: 0 auto;
  padding: 1rem clamp(1rem, 4vw, 2.5rem);
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  flex-wrap: wrap;
}

.brand {
  display: flex;
  align-items: center;
  gap: 0.65rem;
  color: var(--color-heading);
  font-weight: 600;
  font-size: 1.05rem;
}

.brand:hover {
  color: var(--color-heading);
}

.app-nav {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  flex-wrap: wrap;
}

.app-nav a:not(.nav-cta) {
  color: var(--c-text-muted);
  font-weight: 500;
  font-size: 0.9rem;
  padding-bottom: 0.2rem;
  border-bottom: 2px solid transparent;
}

.app-nav a:not(.nav-cta):hover {
  color: var(--c-text);
}

.app-nav a.router-link-exact-active:not(.nav-cta) {
  color: var(--c-blue-300);
  border-bottom-color: var(--c-blue-400);
}

.app-main {
  flex: 1;
  background-image: linear-gradient(rgba(0, 0, 0, 0.7), rgba(0, 0, 0, 0.7)), url('/Stradanous.jpg');
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  background-attachment: fixed;
}

@media (max-width: 480px) {
  .brand-name {
    display: none;
  }
}
</style>
