<template>
  <div>
    <header>
      <h1>Meeting Room Client</h1>
      <nav>
        <router-link to="/">Home</router-link>
      </nav>
    </header>

    <main>
      <router-view />
    </main>
  </div>
</template>

<style scoped>
header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
}

nav a {
  margin-right: 0.5rem;
  color: #2c3e50;
  text-decoration: none;
}

nav a.router-link-active {
  font-weight: 600;
}
</style>
